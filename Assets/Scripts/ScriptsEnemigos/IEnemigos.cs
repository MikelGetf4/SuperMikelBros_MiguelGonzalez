using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    //Interfaz de los enemigos
    public interface IEnemigos
    {
        void RecibirDanio(); //Reciben daño
        void Activar(); //Se activan
    }
}

