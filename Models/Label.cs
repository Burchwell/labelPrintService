using System;

namespace LabelService3.Models
{
    public class Label
    {
        public String id { get; set; }
        public String order_id { get; set; }
        public String pdf_base64 { get; set; }
        public String pdf_url { get; set; }
        public String printed { get; set; }
        public String printed_at { get; set; }
        public String height { get; set; }
        public String width { get; set; }
    }
};