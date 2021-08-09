using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Enum;
using Appical.Domain.Exception;
using Appical.Persistence.Repository.Interface;

namespace Appical.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepo;

        public AccountController(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        /// <summary>
        /// Get an overview of an Account
        /// </summary>
        /// <remarks>
        /// Required permissions: AccountOwner-ViewAccount
        /// </remarks>
        /// <param name="accountOwnerId">Id of the Account to get an Overview for</param>
        /// <returns>An AccountOverviewDto</returns>
        /// <response code="200">Returns the overview on an Account</response>
        /// <response code="400">Validation issues</response>
        [HttpGet("overview/{accountOwnerId}")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountDto>> GetOverviewForAccountOwner(Guid accountOwnerId)
        {
            // Verify request is coming from an AccountOwner that owns the Account

            try
            {
                List<AccountDto> overviewDto = await _accountRepo.ReadForAccountOwner(accountOwnerId);
                return Ok(overviewDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Account with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        /// <summary>
        /// Close Account for an AccountOwner
        /// </summary>
        /// <remarks>
        /// Required permissions: AccountOwner-CloseAccount
        /// </remarks>
        /// <param name="id">Id of the Account to close</param>
        /// <returns>A closed AccountDto</returns>
        /// <response code="200">Returns the reopened AccountDto</response>
        /// <response code="400">Validation issues</response>
        [HttpPut("{id}/Close")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountDto>> CloseAccount(Guid id)
        {
            // Verify request is coming from an AccountOwner that owns the Account being closed

            try
            {
                AccountDto closeDto = await _accountRepo.CloseAccount(id, ClosureReasonType.ClosedByOwner);
                return Ok(closeDto);
            }
            catch (PersistenceEntityDoesNotExistException doesNotExistEx)
            {
                return NotFound($"Account with Id: {doesNotExistEx.Id} does not exist");
            }
            catch (AccountBalanceNotZeroException accountBalanceNotZeroException)
            {
                return BadRequest($"The following Accounts have a balance greater than zero: {string.Join(", ", accountBalanceNotZeroException.AccountIds)}");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}
