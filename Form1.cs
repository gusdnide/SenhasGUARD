using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
namespace Senhas
{

    public partial class Form1 : Form
    {
        Gerenciador ap { get; set; }
        bool SenhaCorreta = false;
        public string SenhaForm()
        {
            frmPass f = new frmPass();
            f.ShowDialog();
            if (f.Retorno == null)
                return "";
            return f.Retorno;
        }
        public Form1(string[] args)
        {
            if (args.Length == 0)
                if (SenhaForm() != Config.Senha)
                    Environment.Exit(1);
                else
                    SenhaCorreta = true;
            else
            {
                if (args.Length == 1 && args[0] == Config.Senha)

                    SenhaCorreta = true;
                else
                    SenhaCorreta = false;                
            }
               
            if (!SenhaCorreta)
                Environment.Exit(1);

            InitializeComponent();
            ap = new Gerenciador();
            Atualizar();
        }

        void Atualizar()
        {
            ap.Carregar();
            listView1.Items.Clear();
            foreach (Gerenciador.Item i in ap.Itens)
            {
                listView1.Items.Add(new ListViewItem(new string[] { i.Nome, i.Usuario, "********" }));
            }
        }

        private void adicionarToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            string Nome = Interaction.InputBox("Digite o nome: ", this.Text);
            string Usuario = Interaction.InputBox("Digite o usuario: ", this.Text);
            string Senha = Interaction.InputBox("Digite a senha: ", this.Text);
            ap.AdicionarItem(Nome, Usuario, Senha);
            Atualizar();
        }

        private void removerToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (listView1.SelectedIndices[0] < 0)
                return;
            ap.RemoverItem(listView1.SelectedIndices[0]);
            Atualizar();
        }

        private void mudarToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (listView1.SelectedIndices[0] < 0)
                return;
            string Nome = Interaction.InputBox("Digite o nome: ", this.Text, ap.Itens[listView1.SelectedIndices[0]].Nome);
            string Usuario = Interaction.InputBox("Digite o usuario: ", this.Text, ap.Itens[listView1.SelectedIndices[0]].Usuario);
            string Senha = Interaction.InputBox("Digite a senha: ", this.Text, ap.Itens[listView1.SelectedIndices[0]].GetSenha());
            ap.EditarItem(listView1.SelectedIndices[0], Nome, Usuario, Senha);
            Atualizar();
        }

        private void usuarioToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (listView1.SelectedIndices[0] < 0)
                return;
            Clipboard.SetText(ap.Itens[listView1.SelectedIndices[0]].Usuario);
        }

        private void senhaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (listView1.SelectedIndices[0] < 0)
                return;
            Clipboard.SetText(ap.Itens[listView1.SelectedIndices[0]].GetSenha());
        }

        private void usuarioSenhaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (listView1.SelectedIndices[0] < 0)
                return;
            Clipboard.SetText(ap.Itens[listView1.SelectedIndices[0]].Usuario + ":" + ap.Itens[listView1.SelectedIndices[0]].GetSenha());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    class Gerenciador
    {
        public struct Item
        {
            public int id { get; set; }
            public string Nome { get; set; }
            public string Usuario { get; set; }
            public string Senha { get; set; }
            public string GetSenha()
            {
                return Cript.Decriptografar(Senha, Config.CriptografiaSenha);
            }
        }

        public List<Item> Itens { get; set; }
        public Gerenciador()
        {
            Itens = new List<Item>();
            Carregar();
        }
        public void AdicionarItem(string n, string u, string s)
        {
            Itens.Add(new Item { id = Itens.Count, Nome = n, Usuario = u, Senha = Cript.Criptografar(s,  Config.CriptografiaSenha)});
            Salvar();
        }
        public void RemoverItem(int id)
        {
            var itm = from i in Itens where (i.id == id) select i;
            if (itm.Count() <= 0) return;
            Itens.Remove(itm.First());
            Salvar();
        }
        public void EditarItem(int i, string n, string u, string s)
        {
            Itens[i] = new Item() { id = i, Nome = n, Usuario = u, Senha = Cript.Criptografar(s, Config.CriptografiaSenha) };
            Salvar();
        }
        public void Salvar()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Itens));
        }
        public void Carregar()
        {
            if (File.Exists("config.json"))
                Itens = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText("config.json"));
        }
    }
    class Cript
    {
        public static string Criptografar(string cs, string cp)
        {
            string Retorno = "";
            for (int i = 0; i < cs.Length; i++)
            {
                if (i < cp.Length)
                {
                    Retorno += (char)((int)cs[i] + (int)cp[i]);
                }
                else
                {
                    Retorno += (char)((int)cs[i] + 15);
                }
            }
            return Retorno;

        }
        public static string Decriptografar(string cs, string cp)
        {
            string Retorno = "";
            for (int i = 0; i < cs.Length; i++)
            {
                if (i < cp.Length)
                {
                    Retorno += (char)((int)cs[i] - (int)cp[i]);
                }
                else
                {
                    Retorno += (char)((int)cs[i] - 15);
                }
            }
            return Retorno;
        }
    }
}
