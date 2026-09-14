using DocumentFormat.OpenXml.Packaging;
using DocuWare.Platform.ServerClient;
using System.Text.RegularExpressions;
using Proyecto_TFG.Models;
using Proyecto_TFG.Services;

namespace Proyecto_TFG.Functions
{
    public class CrearSolicitud
    {

    

        public static void ProcesarSolicitudPedido(int dwDocId, string guidFileCabinet, int docIdModelo, Secrets secrets)
        {
            //Conectar a docuware

            IDocuWareConnect _docuWareConnect = new DocuWareConnect();

            ServiceConnection service = _docuWareConnect.ConnectToDocuWare();
            Document document = service.GetFromDocumentForDocumentAsync(dwDocId, guidFileCabinet).Result;
            //Recupera los datos del archivador

            Dictionary<string, DocumentIndexField> dicDWFields = new Dictionary<string, DocumentIndexField>();
            DocumentIndexField docIdField = DocumentIndexField.Create("IdDocument", document.Id);
            DocumentIndexField docFileCabinet = DocumentIndexField.Create("guidFileCabient", guidFileCabinet);
            dicDWFields.Add("DocId", docIdField);
            dicDWFields.Add("guidFileCabient", docFileCabinet);


            foreach (DocumentIndexField field in document.Fields)
            {

                dicDWFields.Add(field.FieldName, field);

            }
            //service.Disconnect();


            GenerarSolicitudPedido(dicDWFields, docIdModelo, secrets);
        }

        private static void GenerarSolicitudPedido(Dictionary<string, DocumentIndexField> dicDWFields, int docIdModelo, Secrets secrets)
        {
            int docId = int.Parse(ReturnValueField("DocId", dicDWFields));
            string guid = ReturnValueField("guidFileCabient", dicDWFields);
            string modelGuid = "5b2c106d-82a7-4b62-b7a0-ccee24282a6b"; //guid archivador de modelo

            //Cargar Modelo
            IDocuWareConnect _docuWareConnect = new DocuWareConnect();

            ServiceConnection service = _docuWareConnect.ConnectToDocuWare();
            Document document = service.GetFromDocumentForDocumentAsync(docIdModelo, modelGuid).Result;
            var downloadResponse = document.PostToFileDownloadRelationForStreamAsync(
               new FileDownload()
               {
                   TargetFileType = FileDownloadType.Auto
               }).Result;

            Stream fileStream = downloadResponse.Content;
            string fileName = docId.ToString() + "_solicitudPedido.docx";

            using (FileStream outputFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fileStream.CopyTo(outputFileStream);
            }

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(fileName, true))
            {

                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                Regex regexText = null;

                //Datos Proveedor

                docText = ReplaceText("RAZON_SOCIAL_PROVEEDOR", "RAZON_SOCIAL_PROVEEDOR", dicDWFields, docText);
                docText = ReplaceText("CIF_PROVEEDOR", "CIF_PROVEEDOR", dicDWFields, docText);
                docText = ReplaceText("DIRECCION_PROVEEDOR", "DIRECCION_PROVEEDOR", dicDWFields, docText);
                docText = ReplaceText("TELEFONO_PROVEEDOR", "TELEFONO_PROVEEDOR", dicDWFields, docText);
                docText = ReplaceText("EMAIL_PROVEEDOR", "EMAIL_PROVEEDOR", dicDWFields, docText);


                //Datos de Entrega

                docText = ReplaceText("DIRRECION_ENTREGA", "DIRRECION_ENTREGA", dicDWFields, docText);
                docText = ReplaceText("CODIGO_POSTAL", "CODIGO_POSTAL", dicDWFields, docText);
                docText = ReplaceText("PROVINCIA", "PROVINCIA", dicDWFields, docText);
                docText = ReplaceText("MUNICIPIO", "MUNICIPIO", dicDWFields, docText);
                docText = ReplaceText("PAIS", "PAIS", dicDWFields, docText);
                docText = ReplaceText("TELEFONO", "TELEFONO", dicDWFields, docText);
                docText = ReplaceText("FECHA_ENTREGA", "FECHA_ENTREGA", dicDWFields, docText);
                docText = ReplaceText("MODALIDAD_DE_PAGO", "MODALIDAD_DE_PAGO", dicDWFields, docText);

                //Solicitud de Pedido

                docText = ReplaceText("NR_PEDIDO", "NR_PEDIDO", dicDWFields, docText);
                regexText = new Regex("FECHA_FACTURA");
                docText = regexText.Replace(docText, DateTime.Now.ToString("dd/MM/yyyy"));

                //Solicitante

                docText = ReplaceText("RAZON_SOCIAL_RECEPTOR", "RAZON_SOCIAL_RECEPTOR", dicDWFields, docText);
                docText = ReplaceText("CIF_RECEPTOR", "CIF_RECEPTOR", dicDWFields, docText);
                docText = ReplaceText("EMAIL_SOLICITANTE", "EMAIL_SOLICITANTE", dicDWFields, docText);

                //Importes

                docText = ReplaceText("TOTAL_FACTURA", "TOTAL_FACTURA", dicDWFields, docText);

                // Texto que busca para reemplezar las lineas de la solicitud de pedido

                string buscar = @"<w:tr w:rsidR=""007647CC"" w:rsidRPr=""00EA16C5"" w14:paraId=""795D4F14"" w14:textId=""77777777"" w:rsidTr=""0067098D""><w:tc><w:tcPr><w:tcW w:w=""10456"" w:type=""dxa""/><w:gridSpan w:val=""5""/><w:vAlign w:val=""center""/></w:tcPr><w:p w14:paraId=""4054340B"" w14:textId=""7157FDF5"" w:rsidR=""007647CC"" w:rsidRPr=""00AC4AA9"" w:rsidRDefault=""007647CC"" w:rsidP=""007647CC""><w:pPr><w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/><w:jc w:val=""center""/><w:rPr><w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/><w:sz w:val=""16""/><w:szCs w:val=""16""/><w:lang w:val=""es-ES""/></w:rPr></w:pPr><w:proofErr w:type=""spellStart""/><w:r><w:rPr><w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/><w:sz w:val=""16""/><w:szCs w:val=""16""/><w:lang w:val=""es-ES""/></w:rPr><w:t>txtLineaPedido</w:t></w:r><w:proofErr w:type=""spellEnd""/></w:p></w:tc></w:tr>";

                string totalLineas = null;

                #region // Define el formato de la linea de la solicitud del pedido
                string Linea = @"<w:tr w:rsidR=""00EF3180"" w:rsidRPr=""00EA16C5"" w14:paraId=""795D4F14"" w14:textId=""77777777"" w:rsidTr=""000D5CBD"">
				<w:tc>
					<w:tcPr>
						<w:tcW w:w=""1535"" w:type=""dxa""/>
						<w:vAlign w:val=""center""/>
					</w:tcPr>
					<w:p w14:paraId=""7E3B3451"" w14:textId=""6FF3E81B"" w:rsidR=""00316BE8"" w:rsidRPr=""00AC4AA9"" w:rsidRDefault=""00AC4AA9"" w:rsidP=""00AC4AA9"">
						<w:pPr>
							<w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
							<w:jc w:val=""center""/>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
						</w:pPr>
						<w:r>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
							<w:t>LINEA_PRODUCTO</w:t>
						</w:r>
					</w:p>
				</w:tc>
				<w:tc>
					<w:tcPr>
						<w:tcW w:w=""2705"" w:type=""dxa""/>
						<w:vAlign w:val=""center""/>
					</w:tcPr>
					<w:p w14:paraId=""405307C7"" w14:textId=""357A5FBA"" w:rsidR=""00316BE8"" w:rsidRPr=""00AC4AA9"" w:rsidRDefault=""00AC4AA9"" w:rsidP=""00AC4AA9"">
						<w:pPr>
							<w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
						</w:pPr>
						<w:r>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
							<w:t>LINEA_DESCRIPCION_PRODUCTO</w:t>
						</w:r>
					</w:p>
				</w:tc>
				<w:tc>
					<w:tcPr>
						<w:tcW w:w=""1446"" w:type=""dxa""/>
						<w:vAlign w:val=""center""/>
					</w:tcPr>
					<w:p w14:paraId=""4C5E3CFA"" w14:textId=""12E7A271"" w:rsidR=""00316BE8"" w:rsidRPr=""00AC4AA9"" w:rsidRDefault=""00AC4AA9"" w:rsidP=""00AC4AA9"">
						<w:pPr>
							<w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
                            <w:jc w:val=""right""/>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
						</w:pPr>
						<w:r>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
							<w:t>LINEA PRECIO_UNITARIO</w:t>
						</w:r>
					</w:p>
				</w:tc>
				<w:tc>
					<w:tcPr>
						<w:tcW w:w=""2109"" w:type=""dxa""/>
						<w:vAlign w:val=""center""/>
					</w:tcPr>
					<w:p w14:paraId=""56561AEF"" w14:textId=""7B5102DB"" w:rsidR=""00316BE8"" w:rsidRPr=""00AC4AA9"" w:rsidRDefault=""00AC4AA9"" w:rsidP=""00AC4AA9"">
						<w:pPr>
							<w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
							<w:jc w:val=""right""/>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
						</w:pPr>
						<w:r>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
							<w:t>LINEA_CANTIDAD</w:t>
						</w:r>
					</w:p>
				</w:tc>
				<w:tc>
					<w:tcPr>
						<w:tcW w:w=""2661"" w:type=""dxa""/>
						<w:vAlign w:val=""center""/>
					</w:tcPr>
					<w:p w14:paraId=""4054340B"" w14:textId=""099C3A6C"" w:rsidR=""00316BE8"" w:rsidRPr=""00AC4AA9"" w:rsidRDefault=""00AC4AA9"" w:rsidP=""00AC4AA9"">
						<w:pPr>
							<w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
							<w:jc w:val=""right""/>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
						</w:pPr>
						<w:r>
							<w:rPr>
								<w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman""/>
								<w:sz w:val=""16""/>
								<w:szCs w:val=""16""/>
								<w:lang w:val=""es-ES""/>
							</w:rPr>
							<w:t>LINEA_TOTAL_BASE_IMPONIBLE</w:t>
						</w:r>
					</w:p>
				</w:tc>
			</w:tr>
";
                #endregion //Dimensión de linea de solicitud de pedidos

                DocumentIndexFieldTable tableValues = ReturnValueField("LINEAS", dicDWFields);
                if (tableValues.Row.Count > 0)
                {

                    foreach (DocumentIndexFieldTableRow row in tableValues.Row)
                    {

                        totalLineas = totalLineas + AddLineasSolicitudPedido(row, Linea);

                    }
                }

                else
                {

                    totalLineas = "";

                }

                regexText = new Regex(buscar);
                docText = regexText.Replace(docText, totalLineas);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {

                    sw.Write(docText);
                    sw.Flush();
                    sw.Close();

                }
            }

            Document documentpSolicitud = service.GetFromDocumentForDocumentAsync(docId, guid).Result;
            bool uploadFile = DocuWareConnect.DocumentClip(documentpSolicitud, fileName);
            if (uploadFile) { File.Delete(fileName); }

            service.Disconnect();
        }

        private static string AddLineasSolicitudPedido(DocumentIndexFieldTableRow row, string linea)
        {

            string LINEA_PRODUCTO = row.ColumnValue.Find(f => f.FieldName == "LINEA_PRODUCTO")?.Item?.ToString() ?? "";
            string LINEA_DESCRIPCION_PRODUCTO = row.ColumnValue.Find(f => f.FieldName == "LINEA_DESCRIPCION_PRODUCTO")?.Item?.ToString() ?? "";
            string LINEA_PRECIO_UNITARIO = row.ColumnValue.Find(f => f.FieldName == "LINEA_PRECIO_UNITARIO")?.Item?.ToString() ?? "";
            string LINEA_CANTIDAD = row.ColumnValue.Find(f => f.FieldName == "LINEA_CANTIDAD")?.Item?.ToString() ?? "";
            string LINEA_TOTAL_BASE_IMPONIBLE = row.ColumnValue.Find(f => f.FieldName == "LINEA_TOTAL_BASE_IMPONIBLE")?.Item?.ToString() ?? "";

            string lineaReturn = linea.Replace("LINEA_PRODUCTO", LINEA_PRODUCTO).Replace("LINEA_DESCRIPCION_PRODUCTO", LINEA_DESCRIPCION_PRODUCTO)
                .Replace("LINEA PRECIO_UNITARIO", LINEA_PRECIO_UNITARIO).Replace("LINEA_CANTIDAD", LINEA_CANTIDAD).Replace("LINEA_TOTAL_BASE_IMPONIBLE", LINEA_TOTAL_BASE_IMPONIBLE);

            return lineaReturn;

        }

        private static string ReplaceText(string textRelace, string field, Dictionary<string, DocumentIndexField> dicDWFields, string docText)
        {
            // Función auxiliar para escapar caracteres XML
            string EscapeXml(string input)
            {
                return input.Replace("&", "&amp;")
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;")
                            .Replace("\"", "&quot;")
                            .Replace("'", "&apos;");
            }

            // Escapa el texto que quieres reemplazar
            string escapedText = EscapeXml(ReturnValueField(field, dicDWFields));

            Regex regexText = new Regex(textRelace);
            docText = regexText.Replace(docText, escapedText);

            return docText;

        }

        static dynamic ReturnValueField(string key, Dictionary<string, DocumentIndexField> dicDWFields)
        {
            dynamic valueField;
            if (dicDWFields.TryGetValue(key, out var entry) && entry?.Item != null)
            {
                valueField = entry.Item.ToString();
                if (valueField.Contains("Table"))
                {
                    valueField = entry.Item;
                }
            }
            else
            {
                valueField = "";
            }

            return valueField;

        }
    }
    public class dataInput
    {

        public string dwDocId { get; set; }
        public string guidFileCabinet { get; set; }
        public string docIdModelo { get; set; }

    }
}
