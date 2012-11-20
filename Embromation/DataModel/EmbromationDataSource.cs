using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Embromation.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : Embromation.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return EmbromationDataSource.GeraLeroLero(); }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// EmbromationDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class EmbromationDataSource
    {
        private static Random r = new Random();

        private static EmbromationDataSource _EmbromationDataSource = new EmbromationDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _EmbromationDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _EmbromationDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _EmbromationDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public EmbromationDataSource()
        {
            

            var group1 = new SampleDataGroup("Group-1",
                    "Mundo corporativo",
                    "Sua frase de efeito sempre a mão",
                    "Assets/Dilma.png",
                    "É de toda forma imprescindível que analisemos as posições das verticais de nossa atividade maximizando assim, as diretrizes de desenvolvimento sustentável.");
            group1.Items.Add(new SampleDataItem("Group-1-Item-1",
                    "E-mail para equipe",
                    "Confiança na liderança",
                    "Assets/foto1.jpg",
                    "Mostre-se como um chefe superior, seu time  irá achar que você é um guru.",
                    "",
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-2",
                    "Discurso em público",
                    "Conquiste a massa",
                    "Assets/foto2.jpg",
                    "Prepare discursos capazes de curar a insônia da plateia",
                    "",
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-3",
                    "Para o Chefe",
                    "Ganhe um aumento",
                    "Assets/foto3.jpg",
                    "Impressione seu chefe com palavras que vão parecer que você é o cara",
                    "",
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-4",
                    "Para peuniões de status",
                    "Destaque-se na multidão",
                    "Assets/foto4.jpg",
                    "Mostre para todos que tem conteúdo com sua inteligência e desenvoltura",
                    "",
                    group1));
            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                    "Mundo acadêmico",
                    "Frases para engordar seu trabalho",
                    "Assets/foto6.jpg",
                    "Os estudos preconizados apontam para uma mudança de visão no processo de facilitação do conhecimento promovendo assim, uma postura diferente das coligações");
            group2.Items.Add(new SampleDataItem("Group-2-Item-1",
                    "Extatas",
                    "Monografia",
                    "Assets/Matematica.jpg",
                    "Você irá parecer um gênio de extas",
                    "",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-2",
                    "Humanas",
                    "Discursos",
                    "Assets/Humanas.jpg",
                    "Como encontrar explicação pra tudo",
                    "",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-3",
                    "Biológicas",
                    "Introdução",
                    "Assets/felicidade.jpg",
                    "Não sabe o que escrever na monografia?, seus problemas acabaram",
                    "",
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-4",
                    "Técnólogo",
                    "Conclusão de Curso",
                    "Assets/Inteligente.jpg",
                    "Esteja preparado, mostre seu valor, seja o mais inteligente",
                    "",
                    group2));
            this.AllGroups.Add(group2);

        }

        public static string GeraLeroLero()
        {

            String[] tab0 = new String[] {
                "Caros amigos, ",
                "Por outro lado, ",
                "Assim mesmo, ",
                "No entanto, não podemos esquecer que ",
                "Do mesmo modo, ",
                "A prática cotidiana prova que ",
                "Nunca é demais lembrar o peso e o significado destes problemas, uma vez que ",
                "As experiências acumuladas demonstram que ",
                "Acima de tudo, é fundamental ressaltar que ",
                "O incentivo ao avanço tecnológico, assim como ",
                "Não obstante, ",
                "Todas estas questões, devidamente ponderadas, levantam dúvidas sobre se ",
                "Pensando mais a longo prazo, ",
                "O que temos que ter sempre em mente é que ",
                "Ainda assim, existem dúvidas a respeito de como ",
                "Gostaria de enfatizar que ",
                "Todavia, ",
                "A nível organizacional, ",
                "O empenho em analisar ",
                "Percebemos, cada vez mais, que ",
                "No mundo atual, ",
                "É importante questionar o quanto ",
                "Neste sentido, ",
                "Evidentemente, ",
                "Por conseguinte, ",
                "É claro que ",
                "Podemos já vislumbrar o modo pelo qual ",
                "Desta maneira, ",
                "O cuidado em identificar pontos críticos n",
                "A certificação de metodologias que nos auxiliam a lidar com "
            };

            String[] tab1 = new String[]{
                "a execução dos pontos do programa ",
                "a complexidade dos estudos efetuados ",
                "a contínua expansão de nossa atividade ",
                "a estrutura atual da organização ",
                "o novo modelo estrutural aqui preconizado ",
                "o desenvolvimento contínuo de distintas formas de atuação ",
                "a constante divulgação das informações ",
                "a consolidação das estruturas ",
                "a consulta aos diversos militantes ",
                "o início da atividade geral de formação de atitudes ",
                "o desafiador cenário globalizado ",
                "a mobilidade dos capitais internacionais ",
                "o fenômeno da Internet ",
                "a hegemonia do ambiente político ",
                "a expansão dos mercados mundiais ",
                "o aumento do diálogo entre os diferentes setores produtivos ",
                "a crescente influência da mídia ",
                "a necessidade de renovação processual ",
                "a competitividade nas transações comerciais ",
                "o surgimento do comércio virtual ",
                "a revolução dos costumes ",
                "o acompanhamento das preferências de consumo ",
                "o comprometimento entre as equipes ",
                "a determinação clara de objetivos ",
                "a adoção de políticas descentralizadoras ",
                "a valorização de fatores subjetivos ",
                "a percepção das dificuldades ",
                "o entendimento das metas propostas ",
                "o consenso sobre a necessidade de qualificação ",
                "o julgamento imparcial das eventualidades "
            };

            String[] tab2 = new String[]{
                "nos obriga à análise ",
                "cumpre um papel essencial na formulação ",
                "exige a precisão e a definição ",
                "auxilia a preparação e a composição ",
                "garante a contribuição de um grupo importante na determinação ",
                "assume importantes posições no estabelecimento ",
                "facilita a criação ",
                "obstaculiza a apreciação da importância ",
                "oferece uma interessante oportunidade para verificação ",
                "acarreta um processo de reformulação e modernização ",
                "pode nos levar a considerar a reestruturação ",
                "representa uma abertura para a melhoria ",
                "ainda não demonstrou convincentemente que vai participar na mudança ",
                "talvez venha a ressaltar a relatividade ",
                "prepara-nos para enfrentar situações atípicas decorrentes ",
                "maximiza as possibilidades por conta ",
                "desafia a capacidade de equalização ",
                "agrega valor ao estabelecimento ",
                "é uma das consequências ",
                "promove a alavancagem ",
                "não pode mais se dissociar ",
                "possibilita uma melhor visão global ",
                "estimula a padronização ",
                "aponta para a melhoria ",
                "faz parte de um processo de gerenciamento ",
                "causa impacto indireto na reavaliação ",
                "apresenta tendências no sentido de aprovar a manutenção ",
                "estende o alcance e a importância ",
                "deve passar por modificações independentemente ",
                "afeta positivamente a correta previsão "
            };

            String[] tab3 = new String[]{
                "das condições financeiras e administrativas exigidas.",
                "das diretrizes de desenvolvimento para o futuro.",
                "do sistema de participação geral.",
                "das posturas dos órgãos dirigentes com relação às suas atribuições.",
                "das novas proposições.",
                "das direções preferenciais no sentido do progresso.",
                "do sistema de formação de quadros que corresponde às necessidades.",
                "das condições inegavelmente apropriadas.",
                "dos índices pretendidos.",
                "das formas de ação.",
                "dos paradigmas corporativos.",
                "dos relacionamentos verticais entre as hierarquias.",
                "do processo de comunicação como um todo.",
                "dos métodos utilizados na avaliação de resultados.",
                "de todos os recursos funcionais envolvidos.",
                "dos níveis de motivação departamental.",
                "da gestão inovadora da qual fazemos parte.",
                "dos modos de operação convencionais.",
                "de alternativas às soluções ortodoxas.",
                "dos procedimentos normalmente adotados.",
                "dos conhecimentos estratégicos para atingir a excelência.",
                "do fluxo de informações.",
                "do levantamento das variáveis envolvidas.",
                "das diversas correntes de pensamento.",
                "do impacto na agilidade decisória.",
                "das regras de conduta normativas.",
                "do orçamento setorial.",
                "do retorno esperado a longo prazo.",
                "do investimento em reciclagem técnica.",
                "do remanejamento dos quadros funcionais."
            };

            String ITEM_CONTENT = string.Empty;
            Random rLine = new Random();
            for (int i=0; i <= 7; i++)
            {
                lock (r)
                {
                    switch (rLine.Next(1,6))
                    {
                        case 1:
                            ITEM_CONTENT += String.Format("{0}\n\n",
                                tab0[r.Next(tab0.Length - 1)] + " " + tab1[r.Next(tab1.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + ", " + tab2[r.Next(tab2.Length - 1)] + " assim como " + tab1[r.Next(tab2.Length - 1)] + " " + tab3[r.Next(tab3.Length - 1)]);
                            break;
                        case 2:
                            ITEM_CONTENT += String.Format("{0}\n\n",
                                tab0[r.Next(tab0.Length - 1)] + " " + tab1[r.Next(tab1.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + ", " + tab2[r.Next(tab2.Length - 1)] + " assim como " + tab1[r.Next(tab2.Length - 1)] + " " + tab3[r.Next(tab3.Length - 1)]);
                            break;
                        case 3:
                            ITEM_CONTENT += String.Format("{0}\n\n",
                                tab0[r.Next(tab0.Length - 1)] + " " + tab1[r.Next(tab1.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + " e " + tab1[r.Next(tab1.Length - 1)] + ", " + tab2[r.Next(tab2.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + " e " + tab2[r.Next(tab2.Length - 1)] + " assim como " + tab1[r.Next(tab2.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + " " + tab3[r.Next(tab3.Length - 1)]);
                            break;
                        case 4:
                            ITEM_CONTENT += String.Format("{0}\n\n",
                                tab0[r.Next(tab0.Length - 1)] + " " + tab1[r.Next(tab1.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + ", " + tab2[r.Next(tab2.Length - 1)] + " assim como " + tab1[r.Next(tab2.Length - 1)] + " " + tab3[r.Next(tab3.Length - 1)]);
                            break;
                        case 5:
                            ITEM_CONTENT += String.Format("{0}\n\n",
                                tab0[r.Next(tab0.Length - 1)] + " " + tab1[r.Next(tab1.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + ", " + tab2[r.Next(tab2.Length - 1)] + " e " + tab2[r.Next(tab2.Length - 1)] + " " + tab2[r.Next(tab2.Length - 1)] + " assim como " + tab1[r.Next(tab2.Length - 1)] + " " + tab3[r.Next(tab3.Length - 1)]);
                            break;
                        default:
                            ITEM_CONTENT += String.Format("{0}\n\n",
                                tab0[r.Next(tab0.Length - 1)] + " " + tab1[r.Next(tab1.Length - 1)] + " " + " " + tab2[r.Next(tab2.Length - 1)] + " " + tab3[r.Next(tab3.Length - 1)]);
                            break;

                    }
                }
            }

            return ITEM_CONTENT;
        }
    }
}
