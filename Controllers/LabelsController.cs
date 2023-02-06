using LabelService3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LabelService3.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LabelsController : ApiController
    {
        private static List<Label> labels = new List<Label>();

        [HttpGet]
        public IEnumerable<Label> Get()
        {
            return labels.ToList();
        }

        [HttpGet]
        public Label Get(int id)
        {
            try
            {
                return labels[id];
            }
            catch (Exception)
            {
                return new Label();
            }
        }

        [HttpPost]
        public HttpResponseMessage Labels(Label data)
        {
            Console.WriteLine(String.Format("Printing Label for {0}", data.order_id));
            string base64 = PDF.PrintLabelFromUri(data.pdf_url);
            data.pdf_base64 = base64;
            data.printed = "ok";
            data.printed_at = DateTime.Now.ToString("u");
            data.width = "6in";
            data.height = "4in";
            Http.updateNova(data);
            Http.updateCore(data);
            return Request.CreateResponse(data);
        }
    }
}
