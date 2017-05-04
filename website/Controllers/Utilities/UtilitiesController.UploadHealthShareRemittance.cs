using Milton.Website.Controllers.Recon;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Utilities
{
    public partial class UtilitiesController
    {
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadHealthShareRemittance()
        {
            var remittanceFile = this.Request.Files["RemittanceFile"];
            string remittanceDate = this.Request["RemittanceDate"].Replace("/", "");
            DateTime dtRemittanceDate = DateTime.ParseExact(remittanceDate, "MMddyyyy", null);

            var remittanceData = remittanceFile.InputStream.GetBytes();

            string remittanceExtension = Path.GetExtension(remittanceFile.FileName);

            if ((remittanceFile.ContentType == "application/vnd.ms-excel" || remittanceFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") && (remittanceExtension == ".xlsx" || remittanceExtension == ".xls"))
            {
                //Use spreadsheetgear to read the file.
                //If it fails to read then notify the user that the file is invalid.
                IWorkbook reconWorkbook = null;
                try
                {
                    reconWorkbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.OpenFromMemory(remittanceData);
                }
                catch (Exception)
                {
                    throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");
                }

                if (reconWorkbook == null) throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");

                //Validate the number of the required worksheets.
                int ws = reconWorkbook.Worksheets.Count;
                IWorksheet sheet = reconWorkbook.Worksheets[0];

                #region Validate Data
                DataTable reconSheetData = new DataTable();

                List<FileContentException.FileContentError> reconSheetDataIssues = ValidateHealthShareRemittanceSheetData(sheet, out reconSheetData);

                //Join the 2 lists of issues.
                FileContentException dataIssues = new FileContentException();
                dataIssues.Errors.AddRange(reconSheetDataIssues);

                //if (dataIssues != null && dataIssues.Errors.Count > 0) throw dataIssues;
                #endregion Validate Data

                InsertHealthShareRemittanceSheetData(reconSheetData, dtRemittanceDate);
            }
            else
            {
                throw new Exception("Invalid file type uploaded. Please upload an Excel file in the same format as the one provided.");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
		/// Check that the data in the provided sheet matches the data format allowed for the import sheet 1.
		/// </summary>
		/// <param name="sheet"></param>
		/// <param name="importData"></param>
		/// <returns></returns>
		private List<FileContentException.FileContentError> ValidateHealthShareRemittanceSheetData(IWorksheet sheet, out DataTable importData)
        {
            IRange cells = sheet.Cells;
            List<FileContentException.FileContentError> issues = new List<FileContentException.FileContentError>();
            int currentRowIndex = 0;
            int emptyRowTracker = 0;
            int maxEmptyRows = 1;

            //Build the DataTable.
            importData = BuildHealthShareRemittanceSheetDataHolder();

            while (emptyRowTracker < maxEmptyRows)
            {
                //Check if the current row should be skipped.
                bool skipItem = true;

                for (int i = 0; i < importData.Columns.Count; i++)
                    if (!String.IsNullOrEmpty(cells[currentRowIndex, i].Text))
                        skipItem = false;

                if (skipItem)
                {
                    currentRowIndex++;
                    emptyRowTracker++;
                    continue;
                }
                else
                {
                    emptyRowTracker = 0;
                }

                //Validate the column values.
                string invoiceNumber = GetCellValue(cells[currentRowIndex, 1]);
                if (invoiceNumber == null)
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Invoice Number",
                        ColumnNumber = (importData.Columns.IndexOf("InvoiceNumber") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Invoice Number missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });

                string total = GetCellValue(cells[currentRowIndex, 5]);
                Decimal dTotal = 0;
                if (!Decimal.TryParse(total, out dTotal))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Total",
                        ColumnNumber = (importData.Columns.IndexOf("Total") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Total missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                importData.Rows.Add
                    (
                        invoiceNumber,
                        total
                    );

                currentRowIndex++;
            }

            return issues;
        }

        /// <summary>
		/// Bulk insert data into SqlServer
		/// </summary>
		/// <param name="importProduct"></param>
		/// <param name="importProductVariant"></param>
		private void InsertHealthShareRemittanceSheetData(DataTable importRecon, DateTime date)
        {
            int counter1 = 0;
            for (int i = counter1; i < importRecon.Rows.Count; i++)
            {
                string invoiceNumber = importRecon.Rows[i][0].ToString().Replace("-", "");
                decimal amount = Decimal.Parse(importRecon.Rows[i][1].ToString());

                var exist = _paymentService.GetByInvoiceNumber(invoiceNumber);
                if (exist == null)
                {
                    var model = new Database.Models.Finance.Payment()
                    {
                        ActionDate = date,
                        InvoiceNumber = invoiceNumber,
                        Amount = amount
                    };

                    _paymentService.Insert(model);
                }
            }
        }

        /// <summary>
		/// Build a DataTable that can be used for the 1st sheet.
		/// </summary>
		/// <returns></returns>
		private DataTable BuildHealthShareRemittanceSheetDataHolder()
        {
            DataTable table = new DataTable();

            table.Columns.Add("InvoiceNumber");
            table.Columns.Add("Amount");

            return table;
        }
    }
}
