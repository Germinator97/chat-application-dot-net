using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Globalization;

namespace Serveur
{
    class serveur : MarshalByRefObject, Interface.IMethodes
    {

        Hashtable messages = new Hashtable();
        ArrayList clients = new ArrayList();
        string message;
        private int id = 0;

        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(serveur), "serveur", WellKnownObjectMode.Singleton);
            
            Console.WriteLine("Le serveur a bien démarrer.");
            Console.WriteLine("En attente de connexion ...");
            Console.ReadLine();

        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region Connexion et Déconnexion au groupe de discussion
        public string Connexion(string name)
        {

            if (clients.IndexOf(name) > -1)
            {
                return null;
            }

            else
            {

                clients.Add(name);
                sendMsg(name + " s'est connecté(e).");

                return name;

            }

        }

        public void Deconnexion(string name)
        {
            clients.Remove(name);
            sendMsg(name + " s'est déconnecté(e).");
        }
        #endregion

        #region Liste des clients et Id
        public bool monServeur()
        {
            return true;
        }

        public ArrayList getClients()
        {
            return clients;
        }

        public int getId()
        {
            return id;
        }
        #endregion

        #region Envoie de messages
        public void sendMsg(string msg)
        {
            messages.Add(++id, msg);
        }

        public string getMsg(int lastId)
        {

            if (id > lastId)
                return messages[lastId + 1].ToString();

            else
                return null;

        }
        #endregion

    }
}
