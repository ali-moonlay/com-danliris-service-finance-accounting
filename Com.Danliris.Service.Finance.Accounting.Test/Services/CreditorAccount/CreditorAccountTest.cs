﻿using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.CreditorAccount
{
    public class CreditorAccountTest
    {
        private const string ENTITY = "CreditorAccount";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private CreditorAccountDataUtil _dataUtil(CreditorAccountService service)
        {

            GetServiceProvider();
            return new CreditorAccountDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });


            return serviceProvider;
        }

        [Fact]
        public async Task Should_Success_Put_UnitPaymentOrder()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = "UPOputTest";
            data.Code = "UPOcodePutTest";
            data.InvoiceNo = null;
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(newData.InvoiceNo);
            CreditorAccountUnitPaymentOrderPostedViewModel postedData = new CreditorAccountUnitPaymentOrderPostedViewModel()
            {
                InvoiceNo = "InvoiceNo",
                CreditorAccounts = new List<CreditorAccountPostedViewModel>() { newData }
            };
            var updateResponse = await service.UpdateFromUnitPaymentOrderAsync(postedData);
            var updateData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(updateData);
        }

        [Fact]
        public async Task Should_Success_Put_UnitPaymentOrder_Return_0()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            CreditorAccountUnitPaymentOrderPostedViewModel postedData = new CreditorAccountUnitPaymentOrderPostedViewModel()
            {
                InvoiceNo = "InvoiceNo",
                CreditorAccounts = null
            };
            var updateResponse = await service.UpdateFromUnitPaymentOrderAsync(postedData);

            Assert.Equal(0, updateResponse);
        }

        [Fact]
        public async Task Should_Null_Get_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode += "new";
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(newData);
        }

        [Fact]
        public async Task Should_Success_Get_UnitReceiptNote_WithoutInvoiceNo()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.InvoiceNo = null;
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(newData);
        }

        [Fact]
        public async Task Should_Success_Post_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(newData);
        }

        [Fact]
        public async Task Should_Success_Put_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = "putTest";
            data.Code = "codePutTest";
            data.InvoiceNo = null;
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(newData.InvoiceNo);
            newData.InvoiceNo = "InvoiceNo";
            var updateResponse = await service.UpdateFromUnitReceiptNoteAsync(newData);
            var updateData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(updateData);
        }


        [Fact]
        public async Task Should_Fail_Put_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditorAccountUnitReceiptNotePostedViewModel newData = new CreditorAccountUnitReceiptNotePostedViewModel();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateFromUnitReceiptNoteAsync(newData));

        }

        [Fact]
        public async Task Should_Success_Delete_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode += "deleted";
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            var deleteResponse = await service.DeleteFromUnitReceiptNoteAsync(newData.CreditorAccountId);
            var deleteData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(deleteData);
        }

        [Fact]
        public async Task Should_Success_DeleteBy_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.Code = "TestCodeOnly1";
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            var deleteResponse = await service.DeleteFromUnitReceiptNoteAsync(newData);
            var deleteData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(deleteData);
        }

        [Fact]
        public async Task Should_Success_DeleteBy_UnitReceiptNote_Model_NotFound()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var newData = new CreditorAccountUnitReceiptNotePostedViewModel();
            var deleteResponse = await service.DeleteFromUnitReceiptNoteAsync(newData);
            Assert.Equal(0, deleteResponse);
        }

        [Fact]
        public async Task Should_Null_Get_BankExpenditureNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            data.SupplierCode += "new";
            var newData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(newData);
        }


        [Fact]
        public async Task Should_Success_Post_BankExpenditureNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);
            var newData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(newData);
        }

        [Fact]
        public async Task Should_Return_1_IfNotFound()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = "";
            data.SupplierName = "";
            data.InvoiceNo = "";
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);
            var newData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(newData);
        }

        [Fact]
        public async Task Should_Success_Put_BankExpenditureNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);
            var newData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(newData);
            newData.Mutation = 10000;
            var updateResponse = await service.UpdateFromBankExpenditureNoteAsync(newData);
            var updateData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(updateData);
        }

        [Fact]
        public async Task Should_Fail_Put_BankExpenditureNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditorAccountBankExpenditureNotePostedViewModel newData = new CreditorAccountBankExpenditureNotePostedViewModel();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateFromBankExpenditureNoteAsync(newData));

        }

        [Fact]
        public async Task Should_Success_Delete_BankExpenditureNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            unitData.SupplierCode += "deleted";
            unitData.SupplierName += "deleted";
            unitData.InvoiceNo += "deletd";
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);
            var newData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            var deleteResponse = await service.DeleteFromBankExpenditureNoteAsync(newData.CreditorAccountId);
            var deleteData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(deleteData);
        }

        [Fact]
        public async Task Should_Success_Delete_BankExpenditureNoteList()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            unitData.SupplierCode += "deleted";
            unitData.SupplierName += "deleted";
            unitData.InvoiceNo += "deletd";
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);
            var newData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            var deleteResponse = await service.DeleteFromBankExpenditureNoteListAsync(newData.Code);
            var deleteData = await service.GetByBankExpenditureNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(deleteData);
        }

        [Fact]
        public async Task Should_Success_Get_Report()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = service.GetReport(1, 25, data.SupplierName, data.Date.Month, data.Date.Year, 7);
            Assert.NotEmpty(reportResponse.Item1.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Include_Previous_Month()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            data.Mutation = unitData.DPP + unitData.PPN;
            data.Date = data.Date.AddMonths(1);
            var memoData = _dataUtil(service).GetMemoPostedViewModel();
            memoData.SupplierCode = unitData.SupplierCode;
            memoData.SupplierName = unitData.SupplierName;
            memoData.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);
            var nextMonthUnitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            nextMonthUnitData.Date = nextMonthUnitData.Date.AddMonths(1);
            await service.CreateFromUnitReceiptNoteAsync(nextMonthUnitData);
            await service.CreateFromMemoAsync(memoData);
            var reportResponse = service.GetReport(1, 25, data.SupplierName, nextMonthUnitData.Date.Month, nextMonthUnitData.Date.Year, 7);
            Assert.NotEmpty(reportResponse.Item1.Data);
        }

        [Fact]
        public async Task Should_Fail_Create_From_Memo()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditorAccountMemoPostedViewModel memoData = new CreditorAccountMemoPostedViewModel();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateFromMemoAsync(memoData));

        }

        [Fact]
        public async Task Should_Success_Get_Excel()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();

            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            data.Mutation = unitData.DPP + unitData.PPN;

            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);


            var reportResponse = service.GenerateExcel(data.SupplierName, data.Date.Month, data.Date.Year, 7);
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public void Should_Success_Get_Empty_Excel()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var reportResponse = service.GenerateExcel("", 0, 0, 7);
            Assert.NotNull(reportResponse);
        }
    }
}
