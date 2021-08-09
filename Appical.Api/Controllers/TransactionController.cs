using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Exception;
using Appical.Persistence.Repository.Interface;
using Microsoft.Extensions.Logging;

namespace Appical.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
            _logger = logger;
        }

        /// <summary>
        /// Create transaction on Account
        /// </summary>
        /// <remarks>
        /// Required permissions: AccountOwner-MakeTransactions
        /// </remarks>
        /// <param name="dto">AccountTransactionDto - Requirements to alter an account balance</param>
        /// <returns>A newly created TransactionDto</returns>
        /// <response code="201">Returns the newly created TransactionDto</response>
        /// <response code="400">Validation issues</response>
        /// <response code="404">AccountId or AccountOwner Id does not exist</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] AccountTransactionDto dto)
        {
            try
            {
                TransactionDto createdDto = await _transactionRepo.Create(dto);
                return Created($"/Transaction/{createdDto.Id}", createdDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Account or AccountOwner with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (PersistenceEntityNotValidException validationEx)
            {
                return BadRequest(validationEx.Messages);
            }
            catch (AccountClosedException closedAccountEx)
            {
                return BadRequest($"Transactions cannot be performed on a closed Account.Id: {closedAccountEx.Id}");
            }
            catch (InvalidAccountOperationException accountOperationEx)
            {
                return BadRequest($"Could not perform action requested on accountId: {accountOperationEx.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get all Transactions
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ViewAllTransactions
        /// </remarks>
        /// <returns>A list of existing TransactionDtos</returns>
        /// <response code="200">Returns a list of existing TransactionDtos</response>
        /// <response code="204">No Transactions to return</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TransactionDto>>> GetAll()
        {
            try
            {

                List<TransactionDto> transactions = await _transactionRepo.Read();
                if (transactions.Count == 0) return NoContent();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get Transaction for specified Id
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ViewAllTransactions
        /// </remarks>
        /// <param name="id">Guid of the Transaction requested</param>
        /// <returns>A existing (not deleted) TransactionDto with id that is equal to the id passed in the request</returns>
        /// <response code="200">Returns a TransactionDto</response>
        /// <response code="404">Transaction not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
        {
            try
            {

                TransactionDto transactions = await _transactionRepo.Read(id);
                if (transactions == null) return NotFound();

                return Ok(transactions);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Transaction with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get Latest Transaction for specified Account Id
        /// </summary>
        /// <remarks>
        /// Required permissions: AccountOwner-ViewAllTransactions
        /// </remarks>
        /// <param name="accountId">Guid of the Account requested</param>
        /// <returns>The latest TransactionDto performed on the Account for the AccountId supplied</returns>
        /// <response code="200">Returns a TransactionDto</response>
        /// <response code="404">Transaction not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet("Latest/{accountId}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> GetLatestTransactionForAccount(Guid accountId)
        {
            try
            {

                TransactionDto transactions = await _transactionRepo.GetLatestTransactionId(accountId);
                if (transactions == null) return NotFound();

                return Ok(transactions);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Latest Transaction for AccountId: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get Transaction for specified AccountId
        /// </summary>
        /// <remarks>
        /// Required permissions: AccountOwner-ViewAllTransactions
        /// </remarks>
        /// <param name="accountId">Guid of the Account requested</param>
        /// <returns>List of AccountOverviewDtos performed on the Account for the AccountId supplied</returns>
        /// <response code="200">Returns a list of AccountOverviewDtos</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet("List/{accountId}")]
        [ProducesResponseType(typeof(List<AccountOverviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> GetTransactionsForAccount(Guid accountId)
        {
            try
            {

                AccountOverviewDto accountOverviews = await _transactionRepo.ReadForAccount(accountId);
                if (accountOverviews == null) return NotFound();

                return Ok(accountOverviews);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"AccountId: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Update Transaction
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-EditTransaction
        /// </remarks>
        /// <param name="dto">TransactionDto - Uses the Id specified to find which Transaction to edit, other properties will be updated</param>
        /// <returns>The edited TransactionDto</returns>
        /// <response code="200">Returns the edited TransactionDto</response>
        /// <response code="400">Validation issues</response>
        /// <response code="404">Transaction with the specified Id was not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpPut]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction([FromBody] TransactionDto dto)
        {
            try
            {
                TransactionDto createdDto = await _transactionRepo.Update(dto);
                return Ok(createdDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Transaction with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (AccountClosedException closedAccountEx)
            {
                return BadRequest($"Transactions cannot be performed on a closed Account.Id: {closedAccountEx.Id}");
            }
            catch (PersistenceEntityNotValidException validationEx)
            {
                return BadRequest(validationEx.Messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Delete Transaction by Id
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-EditTransaction
        /// </remarks>
        /// <param name="id">Id of an existing Transaction</param>
        /// <returns>The deleted TransactionDto</returns>
        /// <response code="200">Returns the deleted TransactionDto</response>
        /// <response code="400">Validation issues</response>
        /// <response code="404">Transaction with the specified Id was not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> DeleteTransaction(Guid id)
        {
            try
            {
                TransactionDto createdDto = await _transactionRepo.Delete(id);
                return Ok(createdDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Transaction with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}
