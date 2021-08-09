using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.Transaction;
using Appical.Domain.Exception;
using Appical.Persistence.Repository.Interface;

namespace Appical.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepo;

        public TransactionController(ITransactionRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
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
        /// <response code="500">Unhandled exceptions</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] AccountTransactionDto dto)
        {
            try
            {
                TransactionDto createdDto = await _transactionRepo.Create(dto);
                return Created($"/Transaction/{createdDto.Id}", createdDto);
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get Transaction for specified Id
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ViewAllTransactions
        /// </remarks>
        /// <param name="id">Guid of the AccountOwner requested</param>
        /// <returns>A existing (not deleted) TransactionDto with id that is equal to the id passed in the request</returns>
        /// <response code="200">Returns a TransactionDto</response>
        /// <response code="404">Transaction not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet("{id:Guid}")]
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
        [HttpDelete("{id:Guid}")]
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}
