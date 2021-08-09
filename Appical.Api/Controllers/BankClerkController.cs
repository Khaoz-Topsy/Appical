using Appical.Persistence.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appical.Domain.Dto.Account;
using Appical.Domain.Exception;
using Microsoft.AspNetCore.Http;

namespace Appical.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class BankClerkController : ControllerBase
    {
        private readonly IAccountRepository _accountRepo;

        public BankClerkController(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        /// <summary>
        /// Create an Account for an AccountOwner
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-OpenAccount
        /// </remarks>
        /// <param name="accountOwnerId">Id of the AccountOwner to open an account for</param>
        /// <returns>A newly created AccountDto</returns>
        /// <response code="201">Returns the newly created AccountDto</response>
        /// <response code="400">Validation issues</response>
        [HttpPost("{accountOwnerId: guid}/Account")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> OpenAccountForAccountOwner(Guid accountOwnerId)
        {
            try
            {
                AccountDto newAccountDto = await _accountRepo.Create(accountOwnerId);
                return Created($"/Account/{newAccountDto.Id}", newAccountDto);
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
        /// ReOpen Account for an AccountOwner
        /// </summary>
        /// <remarks>
        /// Required permissions: BankClerk-ReOpenAccount
        /// </remarks>
        /// <param name="accountOwnerId">Id of the AccountOwner to re-open an account for</param>
        /// <returns>A re-opened AccountDto</returns>
        /// <response code="200">Returns the reopened AccountDto</response>
        /// <response code="400">Validation issues</response>
        [HttpPut("{accountOwnerId: guid}/ReOpenAccount")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ReOpenAccountForAccountOwner(Guid accountOwnerId)
        {
            throw new NotImplementedException("Out of scope");
        }
    }
}
