using FitnessBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Toestel
    {
        public int Id { get; set; }
		private string beschrijving;

		public string Beschrijving
		{
			get { return beschrijving; }
			set 
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ToestelException("Het toestel moet een beschrijving hebben!");
				}
				else
				{ 
					beschrijving = value; 
				}
			}
		}

	}
}
