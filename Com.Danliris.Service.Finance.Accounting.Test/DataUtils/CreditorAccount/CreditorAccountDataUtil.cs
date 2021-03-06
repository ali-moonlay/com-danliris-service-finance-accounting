﻿using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CreditorAccount
{
    public class CreditorAccountDataUtil
    {
        private readonly CreditorAccountService Service;

        public CreditorAccountDataUtil(CreditorAccountService service)
        {
            Service = service;
        }

        public CreditorAccountUnitReceiptNotePostedViewModel GetUnitReceiptNotePostedViewModel()
        {
            return new CreditorAccountUnitReceiptNotePostedViewModel()
            {
                Code = "Code",
                CreditorAccountId = 1,
                Date = DateTimeOffset.UtcNow,
                DPP = 1000,
                PPN = 100,
                InvoiceNo = "InvoiceNo",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",
                Products = "- Product 1\n- Product 2",
                Currency = "Currency"
            };
        }


        public CreditorAccountBankExpenditureNotePostedViewModel GetBankExpenditureNotePostedViewModel()
        {
            return new CreditorAccountBankExpenditureNotePostedViewModel()
            {
                Code = "Code",
                CreditorAccountId = 1,
                Date = DateTimeOffset.UtcNow,
                Id = 1,
                Mutation = 5500,
                InvoiceNo = "InvoiceNo",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName"
            };
        }

        public CreditorAccountMemoPostedViewModel GetMemoPostedViewModel()
        {
            return new CreditorAccountMemoPostedViewModel()
            {
                Code = "Code",
                CreditorAccountId = 1,
                Date = DateTimeOffset.UtcNow,
                DPP = 1000,
                PPN = 100,
                InvoiceNo = "InvoiceNo",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName"
            };
        }
    }
}
