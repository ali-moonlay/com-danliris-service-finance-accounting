﻿using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Com.Danliris.Service.Production.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.DailyBankTransaction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/daily-bank-transactions")]
    [Authorize]
    public class DailyBankTransactionController : BaseController<DailyBankTransactionModel, DailyBankTransactionViewModel, IDailyBankTransactionService>
    {
        public DailyBankTransactionController(IIdentityService identityService, IValidateService validateService, IDailyBankTransactionService service, IMapper mapper) : base(identityService, validateService, service, mapper, "1.0.0")
        {
        }

        public override async Task<ActionResult> Post([FromBody] DailyBankTransactionViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);

                DailyBankTransactionModel model = Mapper.Map<DailyBankTransactionModel>(viewModel);
                if (viewModel.Status.Equals("OUT", StringComparison.OrdinalIgnoreCase) && viewModel.SourceType.Equals("pendanaan", StringComparison.OrdinalIgnoreCase))
                {

                    await Service.CreateInOutTransactionAsync(model);
                }
                else
                {

                    await Service.CreateAsync(model);
                }

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Utilities.General.CREATED_STATUS_CODE, Utilities.General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationException e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Utilities.General.BAD_REQUEST_STATUS_CODE, Utilities.General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Utilities.General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Utilities.General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("mutation/report")]
        public IActionResult GetReport(int bankId, int month, int year)
        {
            int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
            ReadResponse<DailyBankTransactionModel> Result = Service.GetReport(bankId, month, year, clientTimeZoneOffset);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Result.Data,
                message = Utilities.General.OK_MESSAGE,
                statusCode = Utilities.General.OK_STATUS_CODE
            });
        }

        [HttpGet("mutation/report/download")]
        public IActionResult GetReportXls(int bankId, int month, int year)
        {
            try
            {
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

                var xls = Service.GenerateExcel(bankId, month, year, clientTimeZoneOffset);

                string filename = String.Format("Mutasi Bank Harian - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Utilities.General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Utilities.General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpDelete("by-reference-no/{referenceNo}")]
        public async Task<IActionResult> DeleteByReferenceNo([FromRoute] string referenceNo)
        {
            try
            {
                VerifyUser();

                await Service.DeleteByReferenceNoAsync(referenceNo);

                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Utilities.General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Utilities.General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("daily-balance/report")]
        public IActionResult GetDailyBalanceReport(int bankId, DateTime startDate, DateTime endDate)
        {
            var Result = Service.GetDailyBalanceReport(bankId, startDate, endDate);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Result,
                message = Utilities.General.OK_MESSAGE,
                statusCode = Utilities.General.OK_STATUS_CODE
            });
        }

        [HttpGet("daily-balance/report/download")]
        public IActionResult GetDailyBalanceReportXls(int bankId, DateTime startDate, DateTime endDate)
        {
            try
            {
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

                var xls = Service.GenerateExcelDailyBalance(bankId, startDate, endDate, clientTimeZoneOffset);

                string filename = String.Format("Saldo Bank Harian - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Utilities.General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Utilities.General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
