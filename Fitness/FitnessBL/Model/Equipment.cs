using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Equipment
    {
        public int Equipment_id { get; set; }
        private string device_type;

        public string Device_type
        {
            get { return device_type; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Equals("string"))
                {
                    throw new EquipmentException("Het toestel moet een beschrijving hebben!");
                }
                else
                {
                    device_type = value;
                }
            }
        }

        public Equipment(string beschrijving)
        {
            device_type = beschrijving;
        }

        public Equipment() { }

        public Equipment(int id, string beschrijving)
        {
            Equipment_id = id;
            device_type = beschrijving;
        }

        public override string? ToString()
        {
            return $"EquipmentId: {Equipment_id}, Device type: {device_type}";
        }
    }
}
