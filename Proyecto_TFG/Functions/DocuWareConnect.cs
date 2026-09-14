using DocuWare.Platform.ServerClient;
using Proyecto_TFG.Models;
using Proyecto_TFG.Services;
using System.Text.Json;
using static Proyecto_TFG.Models.DocuWareModelos;

namespace Proyecto_TFG.Functions
{
    public class DocuWareConnect : IDocuWareConnect
    {
        /// <summary>
        /// Realiza la conexión con DocuWare
        /// </summary>
        /// <returns></returns>
        public ServiceConnection ConnectToDocuWare()
        {
            DocuWareSecrets docuWareSecrets = LeerConfConectividad();
            Uri url = new Uri(Uri.UriSchemeHttps + "://" + docuWareSecrets.dwcloud + ".docuware.cloud/DocuWare/Platform/");
            ServiceConnection serviceConnection = ServiceConnection.Create(url, docuWareSecrets.dwUserName, docuWareSecrets.dwPassword);
            return serviceConnection;

        }

        public Document InsertDataToDocuWare(string guidFileCabinet, Dictionary<string,string> datosCabecera)
        {
            FileCabinet fileCabinet = GetFileCabinet(guidFileCabinet, ConnectToDocuWare());

            var indexData = new Document()
            {
                Fields = new List<DocumentIndexField>()
            };

            //Recorrer el dictionary
            foreach (var kv in datosCabecera)
            {
                if(kv.Key.ToString().ToUpper().Trim() != "LINEAS")
                    indexData.Fields.Add(DocumentIndexField.Create(kv.Key, kv.Value));
            }

            if (datosCabecera.ContainsKey("LINEAS")) { //Si datosCabecere contiene la clave Lineas
                string productosJson = datosCabecera["LINEAS"]; //Accedemos al valor de lineas
                List<Producto> lineasProductos = JsonSerializer.Deserialize<List<Producto>>(productosJson); //Deserializamos , cogemos los productos y lo convertimos
                if (lineasProductos != null) {
                    indexData.Fields.Add(CrearTableField(lineasProductos));
                }
            }

            var uploadedDocument = fileCabinet.UploadDocument(indexData);
            return uploadedDocument;
        }

        private FileCabinet GetFileCabinet(string fileCabinetGuid, ServiceConnection serviceConnection)
        {
            return serviceConnection.GetFileCabinet(fileCabinetGuid);
        }

        private DocuWareSecrets LeerConfConectividad()
        {

            DocuWareSecrets docuwareConfig = new DocuWareSecrets()
            {
                dwcloud = "",
                dwPassword = "",
                dwUserName = "",
                guidFileCabinetSolicitud = ""
                
            };


            return docuwareConfig;
        }

        private static DocumentIndexField CrearTableField(List<Producto> lineasProductos)
        {

            DocumentIndexField positionFields = new DocumentIndexField()
            {
                FieldName = "LINEAS",
                ItemElementName = ItemChoiceType.Table,
                Item = new DocumentIndexFieldTable()
                {
                    Row = DocumentIndexFieldTableRowsAdd(lineasProductos)
                }
            };

            return positionFields;
        }

        private static List<DocumentIndexFieldTableRow> DocumentIndexFieldTableRowsAdd(List<Producto> lineasProductos)
        {
            List<DocumentIndexFieldTableRow> documentIndexFieldTableRows = new List<DocumentIndexFieldTableRow>();


            foreach (Producto lineaProducto in lineasProductos)
            {

                DocumentIndexFieldTableRow row = new DocumentIndexFieldTableRow()
                {
                    ColumnValue = new List<DocumentIndexField>()
                    {
                        DocumentIndexField.Create("LINEA_PRODUCTO",lineaProducto.codigo),
                        DocumentIndexField.Create("LINEA_DESCRIPCION_PRODUCTO",lineaProducto.descripcion),
                        DocumentIndexField.Create("LINEA_PRECIO_UNITARIO",lineaProducto.precio_unitario),
                        DocumentIndexField.Create("LINEA_CANTIDAD",lineaProducto.CantidadProducto),
                        DocumentIndexField.Create("LINEA_TOTAL_BASE_IMPONIBLE",lineaProducto.SubtotalProducto)


                    }
                };
                documentIndexFieldTableRows.Add(row);

            }

            Console.WriteLine("");



            return documentIndexFieldTableRows;
        }

        public static bool DocumentClip(Document document, string filePath)
        {
            try
            {
                int totalSeccions = document.SectionCount;
                var result = document.AddDocumentSections(new FileInfo(filePath));
                int newSeccionCount = result.SectionCount;
                if (newSeccionCount == totalSeccions + 1)
                {
                    if (totalSeccions > 1)
                    {
                        for (int i = 1; i < totalSeccions; i++)
                        {
                            string name = filePath.ToString().Trim().ToUpper();
                            if (name == filePath.ToUpper().Trim())
                            {
                                document.Sections[i].DeleteSelfRelation();
                            }

                        }
                    }
                    System.IO.File.Delete(filePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
            return false;
        }




        public static ValidatedDocument ConvertBase64ToFileInfo(string base64String, string fileName)
        {
            ValidatedDocument responseValidatedDocument = new ValidatedDocument();
            FileInfo fileInfo = null;
            try
            {

                // Crear un objeto MemoryStream a partir de la cadena Base64
                MemoryStream stream = ConvertBase64ToMemoryStream(base64String);

                // Crear un objeto FileInfo a partir del MemoryStream
                fileInfo = new FileInfo(fileName);
                using (FileStream fileStream = fileInfo.Create())
                {
                    stream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {

                responseValidatedDocument.response = false;
                responseValidatedDocument.fileInfo = null;
            }

            responseValidatedDocument.response = true;
            responseValidatedDocument.fileInfo = fileInfo;
            return responseValidatedDocument;
        }

        /// <summary>
        /// Transfiere un documento que se recibe en base64 a la memoria
        /// Se utiliza para procesar el documento 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        private static MemoryStream ConvertBase64ToMemoryStream(string base64String)
        {
            byte[] fileBytes = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            return memoryStream;
        }


    }

    //Definir clase independiente solo como modelo
    public class ValidatedDocument
    {

        public bool response { get; set; }
        public FileInfo fileInfo { get; set; }


    }
}
