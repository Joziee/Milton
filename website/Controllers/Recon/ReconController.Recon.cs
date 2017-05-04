using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Recon
{
    public partial class ReconController
    {
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Recon()
        {
            var reconFile = this.Request.Files["ReconFile"];
            var healthShareFile = this.Request.Files["HealthShareFile"];

            var reconData = reconFile.InputStream.GetBytes();
            var healthShareData = healthShareFile.InputStream.GetBytes();

            string reconExtension = Path.GetExtension(reconFile.FileName);
            string healthShareExtension = Path.GetExtension(healthShareFile.FileName);

            if ((reconFile.ContentType == "application/vnd.ms-excel" || reconFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") && (reconExtension == ".xlsx" || reconExtension == ".xls") && (healthShareFile.ContentType == "application/vnd.ms-excel" || healthShareFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") && (healthShareExtension == ".xlsx" || healthShareExtension == ".xls"))
            {
                //Use spreadsheetgear to read the file.
                //If it fails to read then notify the user that the file is invalid.
                IWorkbook reconWorkbook = null;
                IWorkbook healthShareWorkbook = null;
                try
                {
                    reconWorkbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.OpenFromMemory(reconData);
                    healthShareWorkbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.OpenFromMemory(healthShareData);
                }
                catch (Exception)
                {
                    throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");
                }

                if (reconWorkbook == null) throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");
                if (healthShareWorkbook == null) throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");

                //Validate the number of the required worksheets.
                IWorksheet reconSheet = reconWorkbook.Worksheets[0];
                if (reconSheet == null) throw new Exception("Recon Sheet is missing. Please upload an Excel file in the same format as the one provided.");
                IWorksheet healthShareSheet = healthShareWorkbook.Worksheets[0];
                if (healthShareSheet == null) throw new Exception("Health Share Sheet is missing. Please upload an Excel file in the same format as the one provided.");

                #region Validate Data
                DataTable reconSheetData = new DataTable();
                DataTable healthShareSheetData = new DataTable();

                List<FileContentException.FileContentError> reconSheetDataIssues = ValidateReconSheetData(reconSheet, out reconSheetData);
                List<FileContentException.FileContentError> healthShareSheetDataIssues = ValidateHealthShareSheetData(healthShareSheet, out healthShareSheetData);

                //Join the 2 lists of issues.
                FileContentException dataIssues = new FileContentException();
                dataIssues.Errors.AddRange(reconSheetDataIssues);
                dataIssues.Errors.AddRange(healthShareSheetDataIssues);

                if (dataIssues != null && dataIssues.Errors.Count > 0) throw dataIssues;
                #endregion Validate Data

                BulkGenerateReport(reconSheetData, healthShareSheetData);
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
		private List<FileContentException.FileContentError> ValidateReconSheetData(IWorksheet sheet, out DataTable importData)
        {
            IRange cells = sheet.Cells;
            List<FileContentException.FileContentError> issues = new List<FileContentException.FileContentError>();
            int currentRowIndex = 6;
            int emptyRowTracker = 0;
            int maxEmptyRows = 4;

            //Build the DataTable.
            importData = BuildReconSheetDataHolder();

            while (emptyRowTracker < maxEmptyRows)
            {
                //Check if the current row should be skipped.
                bool skipItem = true;

                for (int i = 0; i < importData.Columns.Count; i++)
                    if (cells[currentRowIndex, i] != null && cells[currentRowIndex, i].Value != null && !String.IsNullOrEmpty(cells[currentRowIndex, i].Value.ToString()))
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
                string date = GetCellValue(cells[currentRowIndex, 1]);
                DateTime dtDate = DateTime.MinValue;
                if (!DateTime.TryParse(date, out dtDate))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Date",
                        ColumnNumber = (importData.Columns.IndexOf("Date") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Date missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string invoiceNumber = GetCellValue(cells[currentRowIndex, 2]);
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

                string name = GetCellValue(cells[currentRowIndex, 3]);
                if (name == null)
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Name and Surname",
                        ColumnNumber = (importData.Columns.IndexOf("NameSurname") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Name and Surname missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });

                string debit = GetCellValue(cells[currentRowIndex, 4]);
                decimal debitAmount = 0;
                if (!Decimal.TryParse(debit, out debitAmount))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Amount",
                        ColumnNumber = (importData.Columns.IndexOf("Amount") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Amount missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                if (invoiceNumber != null)
                {
                    importData.Rows.Add
                    (
                        dtDate,
                        invoiceNumber,
                        name,
                        debitAmount
                    );
                }

                currentRowIndex++;
            }

            return issues;
        }

        private List<FileContentException.FileContentError> ValidateHealthShareSheetData(IWorksheet sheet, out DataTable importData)
        {
            IRange cells = sheet.Cells;
            List<FileContentException.FileContentError> issues = new List<FileContentException.FileContentError>();
            int currentRowIndex = 2;
            int emptyRowTracker = 0;
            int maxEmptyRows = 4;

            //Build the DataTable.
            importData = BuildHealthShareSheetDataHolder();

            while (emptyRowTracker < maxEmptyRows)
            {
                //Check if the current row should be skipped.
                bool skipItem = true;

                for (int i = 0; i < importData.Columns.Count; i++)
                    if (cells[currentRowIndex, i] != null && cells[currentRowIndex, i].Value != null && !String.IsNullOrEmpty(cells[currentRowIndex, i].Value.ToString()))
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
                string category = GetCellValue(cells[currentRowIndex, 0]);
                if (!String.IsNullOrEmpty(category)) category = category.Replace("\\", "/");
                if (!String.IsNullOrEmpty(category) && !category.StartsWith("/")) category = "/" + category;

                string manufacturer = GetCellValue(cells[currentRowIndex, 1]);
                string productName = GetCellValue(cells[currentRowIndex, 2]);
                if (productName == null)
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Product Name",
                        ColumnNumber = (importData.Columns.IndexOf("ProductName") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Product Name missing on Sheet 1, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });

                string sku = GetCellValue(cells[currentRowIndex, 3]);
                if (sku == null)
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "SKU",
                        ColumnNumber = (importData.Columns.IndexOf("SKU") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "SKU missing on Sheet 1, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });

                string barcode = GetCellValue(cells[currentRowIndex, 4]);
                string description = GetCellValue(cells[currentRowIndex, 5]);
                string variantRule = GetCellValue(cells[currentRowIndex, 6]);

                #region Validate Prices
                //Check that all the prices are correct if they have a value.
                string price1 = GetCellValue(cells[currentRowIndex, 7]);
                decimal price1Converted = 0;
                bool hasConvertedPrice1 = false;
                FileContentException.FileContentError? price1Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price1") + 1), "Price 1", price1, out price1Converted, out hasConvertedPrice1, out price1);
                if (price1Error != null) issues.Add(price1Error.Value);

                string price2 = GetCellValue(cells[currentRowIndex, 8]);
                decimal price2Converted = 0;
                bool hasConvertedPrice2 = false;
                FileContentException.FileContentError? price2Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price2") + 1), "Price 2", price2, out price2Converted, out hasConvertedPrice2, out price2);
                if (price2Error != null) issues.Add(price2Error.Value);

                string price3 = GetCellValue(cells[currentRowIndex, 9]);
                decimal price3Converted = 0;
                bool hasConvertedPrice3 = false;
                FileContentException.FileContentError? price3Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price3") + 1), "Price 3", price3, out price3Converted, out hasConvertedPrice3, out price3);
                if (price3Error != null) issues.Add(price3Error.Value);

                string price4 = GetCellValue(cells[currentRowIndex, 10]);
                decimal price4Converted = 0;
                bool hasConvertedPrice4 = false;
                FileContentException.FileContentError? price4Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price4") + 1), "Price 4", price4, out price4Converted, out hasConvertedPrice4, out price4);
                if (price4Error != null) issues.Add(price4Error.Value);

                string price5 = GetCellValue(cells[currentRowIndex, 11]);
                decimal price5Converted = 0;
                bool hasConvertedPrice5 = false;
                FileContentException.FileContentError? price5Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price5") + 1), "Price 5", price5, out price5Converted, out hasConvertedPrice5, out price5);
                if (price5Error != null) issues.Add(price5Error.Value);

                string price6 = GetCellValue(cells[currentRowIndex, 12]);
                decimal price6Converted = 0;
                bool hasConvertedPrice6 = false;
                FileContentException.FileContentError? price6Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price6") + 1), "Price 6", price6, out price6Converted, out hasConvertedPrice6, out price6);
                if (price6Error != null) issues.Add(price6Error.Value);

                string price7 = GetCellValue(cells[currentRowIndex, 13]);
                decimal price7Converted = 0;
                bool hasConvertedPrice7 = false;
                FileContentException.FileContentError? price7Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price7") + 1), "Price 7", price7, out price7Converted, out hasConvertedPrice7, out price7);
                if (price7Error != null) issues.Add(price7Error.Value);

                string price8 = GetCellValue(cells[currentRowIndex, 14]);
                decimal price8Converted = 0;
                bool hasConvertedPrice8 = false;
                FileContentException.FileContentError? price8Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price8") + 1), "Price 8", price8, out price8Converted, out hasConvertedPrice8, out price8);
                if (price8Error != null) issues.Add(price8Error.Value);

                string price9 = GetCellValue(cells[currentRowIndex, 15]);
                decimal price9Converted = 0;
                bool hasConvertedPrice9 = false;
                FileContentException.FileContentError? price9Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price9") + 1), "Price 9", price9, out price9Converted, out hasConvertedPrice9, out price9);
                if (price9Error != null) issues.Add(price9Error.Value);

                string price10 = GetCellValue(cells[currentRowIndex, 16]);
                decimal price10Converted = 0;
                bool hasConvertedPrice10 = false;
                FileContentException.FileContentError? price10Error = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Price10") + 1), "Price 10", price10, out price10Converted, out hasConvertedPrice10, out price10);
                if (price10Error != null) issues.Add(price10Error.Value);
                #endregion Validate Prices

                string additionalShippingCost = GetCellValue(cells[currentRowIndex, 17]);
                decimal additionalShippingCostConverted = 0;
                bool hasConvertedAdditionalShippingCost = false;
                FileContentException.FileContentError? additionalShippingCostError = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("AdditionalShippingCost") + 1), "Additional Shipping Cost", additionalShippingCost, out additionalShippingCostConverted, out hasConvertedAdditionalShippingCost, out additionalShippingCost);
                if (additionalShippingCostError != null) issues.Add(additionalShippingCostError.Value);

                string stockQty = GetCellValue(cells[currentRowIndex, 18]);
                decimal stockQtyConverted = 0;
                bool hasConvertedStockQty = false;
                FileContentException.FileContentError? stockQtyError = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("StockQty") + 1), "Stock Qty", stockQty, out stockQtyConverted, out hasConvertedStockQty, out stockQty);
                if (stockQtyError != null) issues.Add(stockQtyError.Value);

                string width = GetCellValue(cells[currentRowIndex, 19]);
                decimal widthConverted = 0;
                bool hasConvertedWidth = false;
                FileContentException.FileContentError? widthError = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Width") + 1), "Width", width, out widthConverted, out hasConvertedWidth, out width);
                if (widthError != null) issues.Add(widthError.Value);

                string height = GetCellValue(cells[currentRowIndex, 20]);
                decimal heightConverted = 0;
                bool hasConvertedHeight = false;
                FileContentException.FileContentError? heightError = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Height") + 1), "Height", height, out heightConverted, out hasConvertedHeight, out height);
                if (heightError != null) issues.Add(heightError.Value);

                string depth = GetCellValue(cells[currentRowIndex, 21]);
                decimal depthConverted = 0;
                bool hasConvertedDepth = false;
                FileContentException.FileContentError? depthError = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Depth") + 1), "Depth", depth, out depthConverted, out hasConvertedDepth, out depth);
                if (depthError != null) issues.Add(depthError.Value);

                string weight = GetCellValue(cells[currentRowIndex, 22]);
                decimal weightConverted = 0;
                bool hasConvertedWeight = false;
                FileContentException.FileContentError? weightError = ValidateDecimalCellValue(1, (currentRowIndex + 1), (importData.Columns.IndexOf("Weight") + 1), "Weight", weight, out weightConverted, out hasConvertedWeight, out weight);
                if (weightError != null) issues.Add(weightError.Value);

                string forSale = GetCellValue(cells[currentRowIndex, 23]);
                string hidden = GetCellValue(cells[currentRowIndex, 24]);
                string featured = GetCellValue(cells[currentRowIndex, 25]);

                importData.Rows.Add
                (
                    !String.IsNullOrEmpty(category) ? category : null,
                    manufacturer,
                    productName,
                    sku,
                    barcode,
                    description,
                    !String.IsNullOrEmpty(variantRule) ? variantRule.Replace("\\", "/") : null,
                    (hasConvertedPrice1 ? price1Converted.ToString() : price1),
                    (hasConvertedPrice2 ? price2Converted.ToString() : price2),
                    (hasConvertedPrice3 ? price3Converted.ToString() : price3),
                    (hasConvertedPrice4 ? price4Converted.ToString() : price4),
                    (hasConvertedPrice5 ? price5Converted.ToString() : price5),
                    (hasConvertedPrice6 ? price6Converted.ToString() : price6),
                    (hasConvertedPrice7 ? price7Converted.ToString() : price7),
                    (hasConvertedPrice8 ? price8Converted.ToString() : price8),
                    (hasConvertedPrice9 ? price9Converted.ToString() : price9),
                    (hasConvertedPrice10 ? price10Converted.ToString() : price10),
                    (hasConvertedAdditionalShippingCost ? additionalShippingCostConverted.ToString() : additionalShippingCost),
                    (hasConvertedStockQty ? stockQtyConverted.ToString() : stockQty),
                    (hasConvertedWidth ? widthConverted.ToString() : width),
                    (hasConvertedHeight ? heightConverted.ToString() : height),
                    (hasConvertedDepth ? depthConverted.ToString() : depth),
                    (hasConvertedWeight ? weightConverted.ToString() : weight),
                    forSale,
                    hidden,
                    featured
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
		private void BulkGenerateReport(DataTable importRecon, DataTable importHealthShare)
        {

        }

        /// <summary>
		/// Build a DataTable that can be used for the 1st sheet.
		/// </summary>
		/// <returns></returns>
		private DataTable BuildReconSheetDataHolder()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Date");
            table.Columns.Add("InvoiceNumber");
            table.Columns.Add("NameSurname");
            table.Columns.Add("Amount");

            DataColumn[] keys = new DataColumn[1];
            keys[0] = table.Columns[1];

            table.PrimaryKey = keys;

            return table;
        }

        /// <summary>
		/// Build a DataTable that can be used for the 1st sheet.
		/// </summary>
		/// <returns></returns>
		private DataTable BuildHealthShareSheetDataHolder()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Date");
            table.Columns.Add("InvoiceNumber");
            table.Columns.Add("NameSurname");
            table.Columns.Add("Amount");

            DataColumn[] keys = new DataColumn[1];
            keys[0] = table.Columns[1];

            table.PrimaryKey = keys;

            return table;
        }

        /// <summary>
		/// Validate a cell and return its value if any.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private string GetCellValue(IRange cell)
        {
            return cell != null
                    && cell.Value != null
                    && !String.IsNullOrEmpty(cell.Value.ToString().Trim())
                    && !cell.Value.ToString().ToLower().Trim().Equals("n/a")
                    && !cell.Value.ToString().ToLower().Trim().Equals("n\a")
                    && !cell.Value.ToString().ToLower().Trim().Equals("n.a") ? cell.Value.ToString().Trim().Replace("'", "''") : null;
        }

        /// <summary>
		/// Try and validate a value that should be a decimal.
		/// </summary>
		/// <param name="sheetNr"></param>
		/// <param name="rowNr"></param>
		/// <param name="colNr"></param>
		/// <param name="columnName"></param>
		/// <param name="value"></param>
		/// <param name="convertedVal"></param>
		/// <param name="hasConvertedVal"></param>
		/// <returns></returns>
		private FileContentException.FileContentError? ValidateDecimalCellValue(int sheetNr, int rowNr, int colNr, string columnName, string value, out decimal convertedVal, out bool hasConvertedVal, out string finalValue)
        {
            //Validate the data.
            value = value != null && value.Equals("-") ? "0" : value;

            //Try and parse the data.
            hasConvertedVal = Decimal.TryParse(value, out convertedVal);

            //Set the value to be outed.
            finalValue = value;

            //If the value cannot be converted, but there is one then it is an invalid value.
            if (!hasConvertedVal && value != null)
                return new FileContentException.FileContentError
                {
                    ColumnName = columnName,
                    ColumnNumber = colNr,
                    ErrorCode = 1,
                    ErrorMessage = "Invalid value for '" + columnName + "' column in Sheet " + sheetNr + ", Row " + rowNr + ". Please update.",
                    RowNumber = rowNr,
                    SheetNumber = 1
                };

            return null;
        }
    }
}
