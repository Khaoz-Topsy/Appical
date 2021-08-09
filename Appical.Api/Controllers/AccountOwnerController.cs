using Appical.Domain.Exception;
using Appical.Persistence.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Dto.AccountOwner;
using Microsoft.Extensions.Logging;

namespace Appical.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AccountOwnerController : ControllerBase
    {
        private readonly IAccountOwnerRepository _accountOwnerRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ILogger<AccountOwnerController> _logger;

        public AccountOwnerController(ILogger<AccountOwnerController> logger, IAccountOwnerRepository accountOwnerRepo, IAccountRepository accountRepo)
        {
            _accountOwnerRepo = accountOwnerRepo;
            _accountRepo = accountRepo;
            _logger = logger;
        }

        /// <summary>
        /// Create an AccountOwner
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-CreateAccounts
        /// </remarks>
        /// <param name="dto">CreateAccountOwnerDto - Requirements to create an account</param>
        /// <returns>A newly created AccountOwnerDto</returns>
        /// <response code="201">Returns the newly created AccountOwnerDto</response>
        /// <response code="400">Validation issues</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpPost]
        [ProducesResponseType(typeof(AccountOwnerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAccountOwner(CreateAccountOwnerDto dto)
        {
            try
            {
                AccountOwnerDto createdDto = await _accountOwnerRepo.Create(dto);
                return Created($"/AccountOwner/{createdDto.Id}", createdDto);
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
        /// Get all AccountOwners
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ViewAllAccountsOwners
        /// </remarks>
        /// <returns>A list of existing (not deleted) AccountOwnerDtos</returns>
        /// <response code="200">Returns a list of existing AccountOwnerDtos</response>
        /// <response code="204">No Accounts to return</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AccountOwnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AccountOwnerDto>>> GetAll()
        {
            try
            {
                List<AccountOwnerDto> accounts = await _accountOwnerRepo.Read();
                if (accounts.Count == 0) return NoContent();

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get AccountOwner by Id
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ViewAllAccountsOwners
        /// </remarks>
        /// <param name="id">Guid of the AccountOwner requested</param>
        /// <returns>A existing (not deleted) AccountOwnerDto with id that is equal to the id passed in the request</returns>
        /// <response code="200">Returns an existing AccountOwnerDto</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountOwnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AccountOwnerDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountOwnerDto>> GetAccountOwner(Guid id)
        {
            try
            {
                AccountOwnerDto account = await _accountOwnerRepo.Read(id);
                if (account == null) return NotFound();

                return Ok(account);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"AccountOwner with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Get Overview of all Accounts for an AccountOwner by Id
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ViewOverview
        /// </remarks>
        /// <param name="id">Guid of the AccountOwner requested</param>
        /// <returns>A list of Accounts associated with the AccountOwner with an id that is equal to the id passed in the request</returns>
        /// <response code="200">Returns a list of accounts associated with the AccountOwner</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpGet("{id}/Overview")]
        [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AccountDto>>> GetAccountOverview(Guid id)
        {
            try
            {
                List<AccountDto> accounts = await _accountRepo.ReadForAccountOwner(id);
                if (accounts == null || accounts.Count == 0) return NoContent();

                return Ok(accounts);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"AccountOwner with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Update AccountOwner
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-EditAccountsOwner
        /// </remarks>
        /// <param name="dto">AccountOwnerDto - Uses the Id specified to find which AccountOwner to edit, other properties will be updated</param>
        /// <returns>The edited AccountOwnerDto</returns>
        /// <response code="200">Returns the edited AccountOwnerDto</response>
        /// <response code="400">Validation issues</response>
        /// <response code="404">AccountOwner with the specified Id was not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpPut]
        [ProducesResponseType(typeof(AccountOwnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountOwnerDto>> UpdateAccountOwner([FromBody] AccountOwnerDto dto)
        {
            try
            {
                AccountOwnerDto createdDto = await _accountOwnerRepo.Update(dto);
                return Ok(createdDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"AccountOwner with Id: {doesNotExistEx.Id} does not exist");
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
        /// Delete AccountOwner by Id
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-EditAccountsOwner
        /// </remarks>
        /// <param name="id">Id of an existing AccountOwner</param>
        /// <returns>The deleted AccountOwnerDto</returns>
        /// <response code="200">Returns the deleted AccountOwnerDto</response>
        /// <response code="400">Validation issues such as accounts not being empty and thus cannot be closed</response>
        /// <response code="404">AccountOwner with the specified Id was not found</response>
        /// <response code="500">Unhandled exceptions</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(AccountOwnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountOwnerDto>> DeleteAccountOwner(Guid id)
        {
            try
            {
                AccountOwnerDto createdDto = await _accountOwnerRepo.Delete(id);
                return Ok(createdDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"AccountOwner with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (AccountBalanceNotZeroException accountBalanceNotZeroException)
            {
                return BadRequest($"AccountOwner has accounts that are not zero: {string.Join(", ", accountBalanceNotZeroException.AccountIds)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}
