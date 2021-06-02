using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace Cliente.Carta
{
    class Cartas
    {
        int numero;
        string tipo;
        string detras;

        List<Cartas> baraja = new List<Cartas>();

        public Cartas(int num, string tip, string detrs)
        {
            this.numero = num;
            this.tipo = tip;
            this.detras = detrs;
        }
         
 

    }
}
