using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.AccountOwner;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Enum;
using Appical.Domain.Exception;
using Appical.Persistence;
using Appical.Persistence.Repository;
using Appical.Persistence.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appical.Test
{
    [TestFixture]
    public class AssessmentTest
    {
        private readonly DbContextOptions<AppicalContext> _dbContextOptions = new DbContextOptionsBuilder<AppicalContext>()
           .UseInMemoryDatabase(databaseName: "Appical")
           .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;

        private IAccountOwnerRepository _accountOwnerRepo;
        private IAccountRepository _accountRepo;
        private ITransactionRepository _transactionRepo;

        private const decimal UseCase2AmountToAdd = 200;
        private const decimal UseCase3AmountToSubtract = -100;

        private Guid _accountOwnerGuid;
        private Guid _accountOwnerTestCreatedAccountGuid;

        [OneTimeSetUp]
        public async Task Setup()
        {
            AppicalContext context = new AppicalContext(_dbContextOptions);
            _accountOwnerRepo = new AccountOwnerRepository(context);
            _accountRepo = new AccountRepository(context);
            _transactionRepo = new TransactionRepository(context);

            CreateAccountOwnerDto newAccountOwner = new CreateAccountOwnerDto { Name = "Tester" };
            AccountOwnerDto createdAccountOwner = await _accountOwnerRepo.Create(newAccountOwner);
            _accountOwnerGuid = createdAccountOwner.Id;

            await context.SaveChangesAsync();
        }

        [TestCase]
        public async Task UseCase1() // Open a new Account
        {
            AccountDto newAccountDto = await _accountRepo.Create(_accountOwnerGuid);

            // --- Acceptance Criteria ---
            // A new Account is created for the specified Account Owner with a Balance of 0.
            Assert.That(newAccountDto.Balance == 0);

            // The new Account has a unique Id
            Assert.NotNull(newAccountDto.Id);
            Assert.That(newAccountDto.Id != _accountOwnerGuid);

            // The new Account has a status of “Open”
            Assert.IsTrue(newAccountDto.Status == AccountStatus.Open);

            _accountOwnerTestCreatedAccountGuid = newAccountDto.Id;
        }

        [TestCase]
        public async Task UseCase2() //  Deposit cash into Account
        {
            TransactionDto latestTransaction = await _transactionRepo.GetLatestTransactionId(_accountOwnerTestCreatedAccountGuid);

            AccountTransactionDto transDto = new AccountTransactionDto
            {
                AccountId = _accountOwnerTestCreatedAccountGuid,
                AccountOwnerId = _accountOwnerGuid,
                Amount = UseCase2AmountToAdd,
                PreviousTransactionId = latestTransaction.Id,
            };
            await _transactionRepo.Create(transDto);

            // --- Acceptance Criteria ---
            // The Deposited Amount is added to the Account Balance
            AccountDto alteredAccountDto = await _accountRepo.Read(_accountOwnerTestCreatedAccountGuid);
            Assert.That(alteredAccountDto.Balance == UseCase2AmountToAdd);
        }

        [TestCase]
        public async Task UseCase3() // Withdraw cash into Account
        {
            TransactionDto latestTransaction = await _transactionRepo.GetLatestTransactionId(_accountOwnerTestCreatedAccountGuid);

            // --- Business Rules ---
            // An account Balance cannot be negative
            AccountTransactionDto expectedToFailTransactionDto = new AccountTransactionDto
            {
                AccountId = _accountOwnerTestCreatedAccountGuid,
                AccountOwnerId = _accountOwnerGuid,
                Amount = -1000000,
                PreviousTransactionId = latestTransaction.Id,
            };
            Assert.ThrowsAsync<PersistenceEntityNotValidException>(async () => await _transactionRepo.Create(expectedToFailTransactionDto));

            // A Withdrawal Transaction either succeeds for the entire amount or fails due to insufficient funds.
            AccountTransactionDto expectedToSucceedTransactionDto = new AccountTransactionDto
            {
                AccountId = _accountOwnerTestCreatedAccountGuid,
                AccountOwnerId = _accountOwnerGuid,
                Amount = UseCase3AmountToSubtract,
                PreviousTransactionId = latestTransaction.Id,
            };
            await _transactionRepo.Create(expectedToSucceedTransactionDto);

            // --- Acceptance Criteria ---
            // The Withdrawal Amount is deducted from the Account Balance
            decimal expectedBalance = UseCase2AmountToAdd + UseCase3AmountToSubtract;
            AccountDto alteredAccountDto = await _accountRepo.Read(_accountOwnerTestCreatedAccountGuid);
            Assert.That(alteredAccountDto.Balance == expectedBalance);
        }

        [TestCase]
        public async Task UseCase4() // Close an Account
        {
            TransactionDto latestTransaction = await _transactionRepo.GetLatestTransactionId(_accountOwnerTestCreatedAccountGuid);

            // --- Business Rules ---
            // An account can only be Closed if the Balance is 0.
            Assert.ThrowsAsync<AccountBalanceNotZeroException>(async () => await _accountRepo.Delete(_accountOwnerTestCreatedAccountGuid));

            // Cash cannot be Deposited into or Withdrawn from a Closed Account.
            decimal expectedBalance = UseCase2AmountToAdd + UseCase3AmountToSubtract;
            AccountTransactionDto expectedToSucceedTransactionDto = new AccountTransactionDto
            {
                AccountId = _accountOwnerTestCreatedAccountGuid,
                AccountOwnerId = _accountOwnerGuid,
                Amount = decimal.Negate(expectedBalance),
                PreviousTransactionId = latestTransaction.Id,
            };
            TransactionDto successfulTransactionThatBringsBalanceToZero = await _transactionRepo.Create(expectedToSucceedTransactionDto);
            Guid latestSuccessfulTransactionId = successfulTransactionThatBringsBalanceToZero.Id;

            AccountDto alteredAccountDto = await _accountRepo.Read(_accountOwnerTestCreatedAccountGuid);
            Assert.That(alteredAccountDto.Balance == 0);

            await _accountRepo.CloseAccount(_accountOwnerTestCreatedAccountGuid, ClosureReasonType.ClosedByOwner);

            AccountTransactionDto expectedToFailTransactionDto = new AccountTransactionDto
            {
                AccountId = _accountOwnerTestCreatedAccountGuid,
                AccountOwnerId = _accountOwnerGuid,
                Amount = 100,
                PreviousTransactionId = latestSuccessfulTransactionId,
            };
            Assert.ThrowsAsync<AccountClosedException>(async () => await _transactionRepo.Create(expectedToFailTransactionDto));

            // --- Acceptance Criteria ---
            // The Account Status is Closed, the Closure Date is recorded and the Account’s Closure Reason is set to "Closed by Owner"
            AccountDto closedAccountDto = await _accountRepo.Read(_accountOwnerTestCreatedAccountGuid);
            Assert.IsTrue(closedAccountDto.Status == AccountStatus.Closed);
            Assert.IsTrue(closedAccountDto.ClosureDate != null);
            Assert.IsTrue(closedAccountDto.ClosureReason == ClosureReasonType.ClosedByOwner);
        }

        [TestCase]
        public async Task UseCase5() // View Accounts overview
        {
            // --- Acceptance Criteria ---
            // The list of all accounts is provided including their
            List<AccountDto> accountSummaries = await _accountRepo.ReadForAccountOwner(_accountOwnerGuid);
            Assert.IsTrue(accountSummaries.Count > 0);
        }

        [TestCase]
        public async Task UseCase6() // View Account transactions
        {
            // --- Acceptance Criteria ---
            // For each transaction the following information is provided
            AccountOverviewDto accountSummary = await _transactionRepo.ReadForAccount(_accountOwnerTestCreatedAccountGuid);
            Assert.IsTrue(accountSummary.Transactions.Count > 0);
        }
    }
}
