using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IMethodes
    {

        string Connexion(string name);

        bool monServeur();

        void Deconnexion(string name);

        ArrayList getClients();

        int getId();

        void sendMsg(string msg);

        string getMsg(int lastId);

    }
}
