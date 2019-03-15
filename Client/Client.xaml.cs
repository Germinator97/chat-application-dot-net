using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Threading;
using Interface;

namespace Client
{
    /// <summary>
    /// Logique d'interaction pour Client.xaml
    /// </summary>
    public partial class client : Window
    {

        internal IMethodes methodes;
        internal int id = 0;
        public string message;
        DispatcherTimer timer = new DispatcherTimer();

        public client()
        {
            InitializeComponent();
            pseudo.Focus();
            timer.Tick += timer_Tick;
        }

        #region Actualisation du client
        public void timer_Tick(object sender, EventArgs e)
        {

            try
            {
                if (methodes.monServeur() == true)
                {
                    
                    message = methodes.getMsg(id);

                    if (message != null)
                    {
                        id++;
                        messagesRecus.Text = messagesRecus.Text + "\n" + string.Format("{0:HH:mm:ss}", DateTime.Now) + " " + message;
                    }

                    clients.ItemsSource = methodes.getClients();

                    if (methodes.getClients().Count < 2)
                    {

                        messageEnvoyes.Text = "Vous ne pouvez entamer une discussion que si vous êtes au moins deux (02) connectés.";
                        
                        return;

                    }

                    else if (messageEnvoyes.Text == "Connectez-vous pour participer aux discussions." || messageEnvoyes.Text == "Vous ne pouvez entamer une discussion que si vous êtes au moins deux (02) connectés.")
                    {

                        messageEnvoyes.IsEnabled = true;
                        messageEnvoyes.Clear();
                        envoyer.IsEnabled = true;
                        
                        return;
                    
                    }

                }
            }

            catch
            {
                
                Deconnexion();
                timer.Stop();

                return;

            }

        }
        #endregion

        #region Connexion et Déconnexion
        public void Connexion()
        {

            if (pseudo.Text.Trim().Length != 0 && ip.Text.Trim().Length != 0 && port.Text.Trim().Length != 0)
            {
               
                try
                {

                    methodes = (IMethodes)Activator.GetObject(typeof(IMethodes), "tcp://" + ip.Text + ":" + port.Text + "/serveur");
                    
                    if (methodes.monServeur() == true)
                    {
                    
                        if (methodes.Connexion(pseudo.Text) == null)
                        {

                            MessageBox.Show("Le pseudo " + "\"" + pseudo.Text + "\"" + " est déjà utilisé, veuillez en choisir un autre.");

                            pseudo.Clear();

                            return;
                        
                        }

                        id = methodes.getId();
                        Title = Title + " " + pseudo.Text;

                        pseudo.IsEnabled = false;
                        ip.IsEnabled = false;
                        port.IsEnabled = false;
                        clients.IsEnabled = true;

                        connexion.Content = "Déconnexion";
                        timer.Start();
                    
                    }

                }

                catch
                {

                    MessageBox.Show("Erreur de connexion, le serveur n'a pas pu être trouvé.");
                    
                    return;
                
                }

            }

            else
            {

                MessageBox.Show("Veuillez remplir les champs pour vous connecter au serveur.");

                pseudo.Focus();

                return;
            
            }

        }

        public void Deconnexion()
        {

            Title = "Client";
            connexion.Content = "Connexion";
            messageEnvoyes.Text = "Connectez-vous pour participer aux discussions.";
            
            pseudo.IsEnabled = true;
            ip.IsEnabled = true;
            port.IsEnabled = true;
            messageEnvoyes.IsEnabled = false;
            clients.IsEnabled = false;
            envoyer.IsEnabled = false;
            clients.ItemsSource = null;
        
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {

            if (connexion.Content.Equals("Connexion"))
            {
                Connexion();
            }

            else
            {

                try
                {
                    if (methodes.monServeur() == true)
                    {

                        Deconnexion();

                        methodes.Deconnexion(pseudo.Text);
                        timer.Stop();
                        
                        return;
                    
                    }
                }

                catch
                {
                    
                    MessageBox.Show("Erreur de connexion, le serveur n'a pas pu être trouvé.");
                    
                    return;
                
                }
                
            }

        }
        #endregion

        #region Envoie de messages
        private void Envoyer_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (methodes.monServeur() == true && messageEnvoyes.Text.Trim().Length > 0)
                {
                    methodes.sendMsg(pseudo.Text + " => " + messageEnvoyes.Text);
                    messageEnvoyes.Text = "";
                }
            }

            catch
            {

                MessageBox.Show("Erreur de connexion, le serveur n'a pas pu être trouvé.");
                
                return;
            
            }
        
        }
        #endregion

        #region Fermer le client
        private void Client_Closed(object sender, EventArgs e)
        {

            try
            {
                if (methodes.monServeur() == true)
                {
                    methodes.Deconnexion(pseudo.Text);
                }
            }

            catch
            {

            }
            
            Close();

        }
        #endregion

    }
}
