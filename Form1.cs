using AsyncUtils;
using DamienG.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MvsxPackBuilder
{
    public partial class Form1 : Form
    {

        public class GamePropertyDisplay
        {
            public string Name { get { return _name; } }
            public string Platform { get { return _platform; } }
            public string Rom { get { return _rom; } }
            public string Manufacturer { get { return _manufacturer; } }
            public string Year { get { return _year; } }

            public string _name = string.Empty;
            public string _platform = string.Empty;
            public string _rom = string.Empty;
            public string _manufacturer = string.Empty;
            public string _year = string.Empty;
        }

        public class AppSettings
        {
            public class InternalSettings
            {
                public string FbaRomsFolder { get { return _fbaRomsFolder; } set { _fbaRomsFolder = value; } }
                public string HyloXFolder { get { return _hyloXFolder; } set { _hyloXFolder = value; } }
                public string LastExportAsFolder { get { return _lastExportAsFolder; } set { _lastExportAsFolder = value; } }
                public string LastCovertArtFolder { get { return _lastCovertArtFolder; } set { _lastCovertArtFolder = value; } }

                private string _fbaRomsFolder = string.Empty;
                private string _hyloXFolder = string.Empty;
                private string _lastExportAsFolder = string.Empty;
                private string _lastCovertArtFolder = string.Empty;
            }

            private InternalSettings _settings = new InternalSettings();

            public string FbaRomsFolder { get { return _settings.FbaRomsFolder; } set { _settings.FbaRomsFolder = value; } }
            public string HyloXFolder { get { return _settings.HyloXFolder; } set { _settings.HyloXFolder = value; } }
            public string LastExportAsFolder { get { return _settings.LastExportAsFolder; } set { _settings.LastExportAsFolder = value; } }
            public string LastCovertArtFolder { get { return _settings.LastCovertArtFolder; } set { _settings.LastCovertArtFolder = value; } }

            private string GetSettingsFilename()
            {
                return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"settings.xml");
            }

            public void LoadSettings()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InternalSettings));

                if (File.Exists(GetSettingsFilename()))
                {
                    using (Stream reader = new FileStream(GetSettingsFilename(), FileMode.Open))
                    {
                        _settings = (InternalSettings)serializer.Deserialize(reader);
                    }
                }
            }

            public void SaveSettings()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InternalSettings));

                using (TextWriter textWriter = new StreamWriter(GetSettingsFilename()))
                {
                    serializer.Serialize(textWriter, _settings);
                }
            }
        }

        public class TreeNodeTag
        {
            public TreeNodeTag(Int32 InFbaGameIndex, Int32 InHyloGameIndex, FBA.SupportedPlatforms InPlatform)
            {
                FbaGameIndex = InFbaGameIndex;
                HyloGameIndex = InHyloGameIndex;
                Platform = InPlatform;
            }

            public Int32 FbaGameIndex;
            public Int32 HyloGameIndex;
            public FBA.SupportedPlatforms Platform;
        }

        private string RomdatPath = Path.Combine(@"data", @"fba_romdat");
        private string PlaceholderCoverArtPath = Path.Combine("data","default_cover.png");
        private string PlaceholderBackgroundPath = Path.Combine("data", "default_bg.png");
        private string PlaceholderIndicatorPath = Path.Combine("data", "default_indicator.png");
        private string ImageMaskPath = Path.Combine("data", "image_mask.png");

        private FBA FbaRomset = new FBA();
        private Hylo HyloHack = new Hylo();

        private void RefreshTreeView(in TreeNode rootNode, TreeView treeView)
        {
            // Display a wait cursor while the TreeNodes are being created.
            Cursor.Current = Cursors.WaitCursor;

            // Suppress repainting the TreeView until all the objects have been created.
            treeView.BeginUpdate();

            // Clear the TreeView each time the method is called.
            treeView.Nodes.Clear();

            // add the new entries
            if(rootNode != null && rootNode.Nodes.Count > 0)
            {
                TreeNode[] RootNodeArray = new TreeNode[rootNode.Nodes.Count];
                rootNode.Nodes.CopyTo(RootNodeArray, 0);
                treeView.Nodes.AddRange(RootNodeArray);
            }

            // Reset the cursor to the default for all controls.
            Cursor.Current = Cursors.Default;

            // Begin repainting the TreeView.
            treeView.EndUpdate();
        }


        private void AddFbaGameToTreeView(FBA.SupportedPlatforms platform, Int32 GameIndex, ref Dictionary<int, int> GameIndexToTreeNodeIndex, TreeNode rootNode)
        {
            game theGame = FbaRomset.GetGameFromIndex(platform, GameIndex);

            TreeNode theTreeNode = new TreeNode(theGame.description);
            theTreeNode.Name = theGame.name;
            theTreeNode.Tag = new TreeNodeTag(GameIndex, -1, platform);

            FBA.FbaGameMetadata gameMetaData = FbaRomset.GetFbaGameMetadataFromIndex(platform, GameIndex);

            if (gameMetaData != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(gameMetaData.ValidationLog);
                builder.Append(gameMetaData.CrcLog);

                theTreeNode.ToolTipText = builder.ToString();
                if (!gameMetaData.DoesRomFileExists)
                {
                    theTreeNode.NodeFont = TreeViewStrikeoutFont;
                }
                if(!gameMetaData.DoesRomCrcMatch)
                {
                    theTreeNode.ForeColor = Color.Red;
                }
            }

            GameIndexToTreeNodeIndex.Add(GameIndex, rootNode.Nodes.Add(theTreeNode));
        }

        private TreeNode PopulateFBATreeView(FBA.SupportedPlatforms platform)
        {
            TreeNode rootNode = new TreeNode();

            Dictionary<int, int> DatIndexToTreeNodeIndex = new Dictionary<int, int>();

            datafile currentDatFile = FbaRomset.GetDatafileFromPlatorm(platform);

            // populate the list view 
            for (Int32 GameIndex = 0; GameIndex < currentDatFile.game.Length; ++GameIndex)
            {
                game theGame = currentDatFile.game[GameIndex];

                // find the parent if any
                if (theGame.cloneof != null)
                {
                    // get my parent index
                    int ParentGameIndex = FbaRomset.GetFbaGameIndexFromRomName(platform, theGame.cloneof);

                    // check that the parent exists
                    if (DatIndexToTreeNodeIndex.ContainsKey(ParentGameIndex) == false)
                    {
                        // parent has not been added, then add it 
                        AddFbaGameToTreeView(platform, ParentGameIndex, ref DatIndexToTreeNodeIndex, rootNode);
                    }

                    // add the clone as a child
                    int ParentTreeNodeIndex = DatIndexToTreeNodeIndex[ParentGameIndex];

                    AddFbaGameToTreeView(platform, GameIndex, ref DatIndexToTreeNodeIndex, rootNode.Nodes[ParentTreeNodeIndex]);
                }
                else
                {
                    // make sure we have not already been added 
                    if (!DatIndexToTreeNodeIndex.ContainsKey(GameIndex))
                    {
                        AddFbaGameToTreeView(platform, GameIndex, ref DatIndexToTreeNodeIndex, rootNode);
                    }
                }
            }

            return rootNode;
        }

        private TreeNode PopulateHyloTreeView(List<Hylo.GameEntry> GameEntries)
        {
            TreeNode rootNode = new TreeNode();

            for(Int32 hyloGameIndex = 0; hyloGameIndex < GameEntries.Count; ++hyloGameIndex)
            {
                Hylo.GameEntry entry = GameEntries[hyloGameIndex];

                string gameName = entry.Name.Trim();
                string datGameName = entry.Dir;

                int datGameIndex;
                Dictionary<string, int> NameToDatIndexMapping = FbaRomset.GetNameToDatIndexMapping(entry.Platform);
                if (NameToDatIndexMapping.TryGetValue(entry.GameName, out datGameIndex))
                {
                    game datGame = FbaRomset.GetGameFromIndex(entry.Platform, datGameIndex);
                    datGameName = datGame.description;
                }

                if (string.IsNullOrEmpty(gameName))
                {
                    gameName = datGameName;
                }

                TreeNode theTreeNode = new TreeNode(gameName);
                theTreeNode.Name = entry.GameName;
                theTreeNode.Tag = new TreeNodeTag(datGameIndex, hyloGameIndex, FBA.GetPlaformFromDirName(entry.Dir));
                rootNode.Nodes.Add(theTreeNode);
            }

            return rootNode;
        }

        private TreeNode RootNodeFbaView;
        private TreeNode RootNodeHyloView;
        private FBA.SupportedPlatforms CurrentPlatform;

        AppSettings Settings = new AppSettings();

        Font TreeViewStrikeoutFont;


        public Form1()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.HyloIcon;

            // disable all the category button until we have loaded a valud HyloX installs
            this.CategorySettingButton1.Enabled = false;
            this.AddCategoryButton.Enabled = false;
            this.RemoveCategoryButton.Enabled = false;

            // set the placeholder image
            pictureBox1.ImageLocation = PlaceholderCoverArtPath;

            // create a font with strike out if we are missing some roms
            TreeViewStrikeoutFont = new Font(fba_treeView1.Font, FontStyle.Strikeout);

            // load all the romdats from the data folder.
            FbaRomset.LoadAllRomdats(RomdatPath);

            // default to arcade?
            CurrentPlatform = FBA.SupportedPlatforms.Arcade;

            Settings.LoadSettings();
            LoadMVSXFromFolder(Settings.HyloXFolder);
            LoadFbaRomsFromFolder(Settings.FbaRomsFolder);

            // fill the platform combo box
            {
                FbaSupportedPlatformComboBox.DataSource = Enum.GetValues(typeof(FBA.SupportedPlatforms));
                FbaSupportedPlatformComboBox.SelectedItem = CurrentPlatform;
            }
            
            // manually bind some events to help with right click
            fba_treeView1.NodeMouseClick += treeView_NodeMouseClick;
            Hylo_treeView2.NodeMouseClick += treeView_NodeMouseClick;

            // TODO: have this on a thread, and it needs to be user initiated.
            // once we have run the process, keep a manifest that keep the last uptodate metadata on the zip file.
#if VALIDATE_CRC
            {
                List<string> validationLog;
                List<int> invalidGameIndices;
                ValidateRomsCRC(currentPlatform, out validationLog, out invalidGameIndices);
            }
#endif
        }


        private void SetGameInPropertyView(FBA.SupportedPlatforms Platform, Int32 FbaGameIndex)
        {
            game foundGame = FbaRomset.GetGameFromIndex(Platform, FbaGameIndex);

            GamePropertyDisplay display = new GamePropertyDisplay();
            display._name = foundGame.description;
            display._platform = Platform.ToString();
            display._rom = foundGame.name;
            display._manufacturer = foundGame.manufacturer;
            display._year = foundGame.year;

            propertyGrid1.SelectedObject = display;
        }



        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNodeTag theTag = e.Node.Tag as TreeNodeTag;
            //propertyGrid1.SelectedObject = FbaRomset.GetGameFromIndex(theTag.Platform, theTag.FbaGameIndex);
            SetGameInPropertyView(theTag.Platform, theTag.FbaGameIndex);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                RootNodeFbaView = PopulateFBATreeView((FBA.SupportedPlatforms)comboBox.SelectedItem);
                RefreshTreeView(RootNodeFbaView, fba_treeView1);
            }
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNodeTag theTag = e.Node.Tag as TreeNodeTag;

            game foundGame = FbaRomset.GetGameFromIndex(theTag.Platform, theTag.FbaGameIndex);
            SetGameInPropertyView(theTag.Platform, theTag.FbaGameIndex);

            // always default to the placeholder one.
            string coverArtPath = PlaceholderCoverArtPath;

            string customCoverArtPath = string.Empty;
            // if it doesn't exits, did we set one manually?
            Hylo.Category selectedCategory = (Hylo.Category)HylostickGameIniComboBox.SelectedItem;
            if (selectedCategory != null)
            {
                customCoverArtPath = selectedCategory.Entries[theTag.HyloGameIndex].CustomCoverPath;
                if (File.Exists(customCoverArtPath))
                {
                    coverArtPath = customCoverArtPath;
                }
                else
                {
                    // find the already exported cover art.
                    string exportedCoverArtPath = Path.Combine(Hylo.GetCoverArtPath(Settings.HyloXFolder), foundGame.name, Hylo.CoverArtFilename);

                    if (File.Exists(exportedCoverArtPath))
                    {
                        coverArtPath = exportedCoverArtPath;
                    }
                }
            }

            pictureBox1.ImageLocation = coverArtPath;
        }

        private void HylostickGameIni_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                // switch game ini
                Hylo.Category newCategory = (Hylo.Category)comboBox.SelectedItem;

                // populate the view
                {
                    RootNodeHyloView = PopulateHyloTreeView(newCategory.Entries);
                    RefreshTreeView(RootNodeHyloView, Hylo_treeView2);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // filter nodes 
            TreeNode newRootNode = new TreeNode();
            FilterNodes(textBox.Text, in RootNodeFbaView, ref newRootNode);
            RefreshTreeView(newRootNode, fba_treeView1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // filter nodes 
            TreeNode newRootNode = new TreeNode();
            FilterNodes(textBox.Text, in RootNodeHyloView, ref newRootNode);
            RefreshTreeView(newRootNode, Hylo_treeView2);
        }

        private void FilterNodes(string textFilter, in TreeNode rootNode, ref TreeNode newRootNode)
        {
            foreach(TreeNode node in rootNode.Nodes)
            {
                TreeNode cloneNode = node.Clone() as TreeNode;
                cloneNode.Nodes.Clear();

                // check the children first
                FilterNodes(textFilter, node, ref cloneNode);

                if (node.Text.Contains(textFilter, StringComparison.OrdinalIgnoreCase) || cloneNode.Nodes.Count > 0)
                {
                    newRootNode.Nodes.Add(cloneNode);
                }
            }
        }

        private void ResetHyloX()
        {
            HyloHack.Reset();

            // clear the combo box                    
            HylostickGameIniComboBox.DataSource = null;
            RootNodeHyloView = null;
            RefreshTreeView(RootNodeHyloView, Hylo_treeView2);

            this.CategorySettingButton1.Enabled = false;
            this.AddCategoryButton.Enabled = false;
            this.RemoveCategoryButton.Enabled = false;
        }


        private void UpdateFormTitle()
        {
            // set the title bar
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            this.Text = string.Format("{0:s} | {1:s}", assemblyName.Name, Settings.HyloXFolder);
        }


        private void LoadMVSXFromFolder(string folderName)
        {
            if (folderName != Settings.HyloXFolder)
            {
                Settings.HyloXFolder = folderName;
                Settings.SaveSettings();
            }

            UpdateFormTitle();

            ResetHyloX();

            if (string.IsNullOrEmpty(folderName))
                return;

            // look for the category ini 
            HyloHack.Categories = HyloCategoryParser.Deserialize(Hylo.GetLanguageIni(Settings.HyloXFolder));

            for(Int32 CategoryIndex = 0; CategoryIndex < HyloHack.Categories.Count; ++CategoryIndex)
            {
                Hylo.Category Category = HyloHack.Categories[CategoryIndex];
                Category.CustomBackgroundPath = Path.Combine(Hylo.GetBackgroundPath(Settings.HyloXFolder), string.Format(Hylo.BackgroundImageFormat, CategoryIndex));
                Category.CustomIndicatorPath = Path.Combine(Hylo.GetIndicatorPath(Settings.HyloXFolder), string.Format(Hylo.IndicatorFolderFormat, CategoryIndex), Hylo.IndicatorImage);

                string gamesIniFilename = Hylo.GetGamesIniFromCategoryIndex(Settings.HyloXFolder, CategoryIndex);
                Category.Entries = HyloGameIniParser.Deserialize(gamesIniFilename);

                // marshall the data so that I can know which game I am referencing, with the correct platform information
                foreach(Hylo.GameEntry entry in Category.Entries)
                {
                    entry.Platform = FBA.GetPlaformFromDirName(entry.Dir);
                    entry.GameName = FBA.GetGameNameWithoutPlatform(entry.Platform, entry.Dir);
                    entry.FbaGameIndex = FbaRomset.GetFbaGameIndexFromRomName(entry.Platform, entry.GameName);
                    entry.CustomCoverPath = Path.Combine(Hylo.GetCoverArtPath(Settings.HyloXFolder), FBA.GetGameNameWithPlatform(entry.Platform, entry.GameName), Hylo.CoverArtFilename);
                }
            }

            // clear the combo box                    
            HylostickGameIniComboBox.DataSource = null;
            RootNodeHyloView = null;

            // populate the view 
            {
                List<Hylo.GameEntry> gameEntries = new List<Hylo.GameEntry>();

                if(HyloHack.Categories.Count > 0)
                {
                    gameEntries = HyloHack.Categories[0].Entries;
                }

                RootNodeHyloView = PopulateHyloTreeView(gameEntries);
                RefreshTreeView(RootNodeHyloView, Hylo_treeView2);

                // populate the combo box
                HylostickGameIniComboBox.DataSource = null;

                if (HyloHack.Categories.Count > 0)
                {
                    HylostickGameIniComboBox.DataSource = HyloHack.Categories;
                    HylostickGameIniComboBox.DisplayMember = "DisplayNameWithCount";
                    HylostickGameIniComboBox.SelectedIndex = 0;
                }


                this.CategorySettingButton1.Enabled = true;
                this.AddCategoryButton.Enabled = true;
                this.RemoveCategoryButton.Enabled = true;
            }
        }

        private void openMVSXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open a file dialog
            using (var fbd = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(Settings.HyloXFolder))
                {
                    fbd.SelectedPath = Settings.HyloXFolder;
                }
                DialogResult result = fbd.ShowDialog();
            
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    LoadMVSXFromFolder(fbd.SelectedPath);
                }
            }
        }

        private void LoadFbaRomsFromFolder(string folderName)
        {
            if (folderName != Settings.FbaRomsFolder)
            {
                Settings.FbaRomsFolder = folderName;
                Settings.SaveSettings();
            }

            // check that we have the files
            {
                StringBuilder validationLog;
                List<int> missingGameIndices;
                FbaRomset.ValidateRomsExists(Settings.FbaRomsFolder, out validationLog, out missingGameIndices);

                // refresh the view
                RootNodeFbaView = PopulateFBATreeView(CurrentPlatform);
                RefreshTreeView(RootNodeFbaView, fba_treeView1);
            }
        }

        private void openFbaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(Settings.FbaRomsFolder))
                {
                    fbd.SelectedPath = Settings.FbaRomsFolder;
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    LoadFbaRomsFromFolder(fbd.SelectedPath);
                }
            }
        }

        private void Hylo_RemoveRomMenuItem_Click(object sender, EventArgs e)
        {
            if (HylostickGameIniComboBox.SelectedItem != null)
            {
                TreeNodeTag theTag = Hylo_treeView2.SelectedNode.Tag as TreeNodeTag;

                Hylo.Category selectedCategory = (Hylo.Category)HylostickGameIniComboBox.SelectedItem;
                selectedCategory.Entries.RemoveAt(theTag.HyloGameIndex);

                // refresh the view
                RootNodeHyloView = PopulateHyloTreeView(selectedCategory.Entries);
                RefreshTreeView(RootNodeHyloView, Hylo_treeView2);

                // clear the previous selection
                Hylo_treeView2.SelectedNode = null;

                // always default to the placeholder one.
                string coverArtPath = PlaceholderCoverArtPath;
                pictureBox1.ImageLocation = coverArtPath;

                // and empty the grid view.
                propertyGrid1.SelectedObject = null;

                // refresh the combo box
                HylostickGameIniComboBox.DataSource = null;
                HylostickGameIniComboBox.DataSource = HyloHack.Categories;
                HylostickGameIniComboBox.DisplayMember = "DisplayNameWithCount";
            }
        }

        private void FBA_AddRomMenuItem_Click(object sender, EventArgs e)
        {
            TreeNodeTag theTag = fba_treeView1.SelectedNode.Tag as TreeNodeTag;
            game foundGame = FbaRomset.GetGameFromIndex(theTag.Platform, theTag.FbaGameIndex);

            // add to the currently selected ini
            if (HylostickGameIniComboBox.SelectedItem != null)
            {
                Hylo.Category selectedCategory = (Hylo.Category)HylostickGameIniComboBox.SelectedItem;

                string newEntryDir = FBA.GetGameNameWithPlatform(theTag.Platform, foundGame.name);

                // check that this entry is not already in the list 
                foreach (Hylo.GameEntry entry in selectedCategory.Entries)
                {
                    if(entry.Dir == newEntryDir)
                    {
                        return;
                    }
                }

                Hylo.GameEntry newEntry = new Hylo.GameEntry();
                newEntry.ID = selectedCategory.Entries.Count;
                newEntry.Dir = newEntryDir;
                newEntry.Name = foundGame.description;
                newEntry.Time = foundGame.year;

                newEntry.CustomCoverPath = string.Empty;
                newEntry.Platform = theTag.Platform;
                newEntry.FbaGameIndex = theTag.FbaGameIndex;
                newEntry.GameName = foundGame.name;

                selectedCategory.Entries.Add(newEntry);

                // refresh the view
                RootNodeHyloView = PopulateHyloTreeView(selectedCategory.Entries);
                RefreshTreeView(RootNodeHyloView, Hylo_treeView2);

                // refresh the combo box
                HylostickGameIniComboBox.DataSource = null;
                HylostickGameIniComboBox.DataSource = HyloHack.Categories;
                HylostickGameIniComboBox.DisplayMember = "DisplayNameWithCount";

                // make sure the hylo tree view is in focus.
                Hylo_treeView2.Focus();

                // make sure the added node is now active
                System.Windows.Forms.TreeNode[] foundNodes = Hylo_treeView2.Nodes.Find(newEntry.GameName, false);
                if(foundNodes.Length != 0)
                {
                    Hylo_treeView2.SelectedNode = foundNodes[0];
                }
            }
        }

        void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                e.Node.TreeView.SelectedNode = e.Node;
        }

        private Int32 GetAllGamesToExportCount()
        {
            Int32 Count = 0;
            foreach(Hylo.Category Category in HyloHack.Categories)
            {
                Count += Category.Entries.Count;
            }

            return Count;
        }

        private void ExportHyloToFolder(string targetFolder, IProgress<int> progress, CancellationToken cancellationToken)
        {
            Int32 ProcessedCount = 0;

            // save the language.ini
            HyloCategoryParser.Serialize(Hylo.GetLanguageIni(targetFolder), HyloHack.Categories);

            for (Int32 categoryIndex = 0; categoryIndex < HyloHack.Categories.Count; ++categoryIndex)
            {
                Hylo.Category category = HyloHack.Categories[categoryIndex];

                #region Process Font
                // create the folder for the font
                string CategoryFolder = Path.Combine(Hylo.GetLocalPath(targetFolder), category.FolderName);
                if (!Directory.Exists(CategoryFolder))
                {
                    Directory.CreateDirectory(CategoryFolder);
                }

                // copy default font if it does not exists
                string fontPath = Path.Combine(CategoryFolder, Hylo.DefaultFontName);
                if(!File.Exists(fontPath))
                {
                    // get the default installed font from english
                    string defaultEnglishFontPath = Path.Combine(Hylo.GetLocalPath(Settings.HyloXFolder), Hylo.EnglishFolder, Hylo.DefaultFontName);
                    File.Copy(defaultEnglishFontPath, fontPath);
                }
                #endregion

                #region Process Category Background
                // process the background
                {
                    string backgroundFolder = Hylo.GetBackgroundPath(targetFolder);

                    // make sure the directory exists
                    if (!Directory.Exists(backgroundFolder))
                    {
                        Directory.CreateDirectory(backgroundFolder);
                    }

                    string targetBackgroundFilename = Path.Combine(backgroundFolder, string.Format(Hylo.BackgroundImageFormat, categoryIndex));

                    if (!string.IsNullOrEmpty(category.CustomBackgroundPath))
                    {
                        if (targetBackgroundFilename != category.CustomBackgroundPath)
                        {
                            ExportResizedImageKeepAspectRatio(1280, 1024, category.CustomBackgroundPath, targetBackgroundFilename);
                        }
                    }

                    if (!File.Exists(targetBackgroundFilename))
                    {
                        // copy the placeholder one from the data folder
                        File.Copy(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), PlaceholderBackgroundPath), targetBackgroundFilename);
                    }
                }
                #endregion

                #region Process Indicator
                {
                    string indicatorFolder = Path.Combine(Hylo.GetIndicatorPath(targetFolder), string.Format(Hylo.IndicatorFolderFormat, categoryIndex));

                    // make sure the directory exists
                    if (!Directory.Exists(indicatorFolder))
                    {
                        Directory.CreateDirectory(indicatorFolder);
                    }

                    string targetIndicatorFilename = Path.Combine(indicatorFolder, Hylo.IndicatorImage);

                    if (!string.IsNullOrEmpty(category.CustomIndicatorPath))
                    {
                        if (targetIndicatorFilename != category.CustomIndicatorPath)
                        {
                            ExportResizedImageKeepAspectRatio(286, 344, category.CustomIndicatorPath, targetIndicatorFilename);
                        }
                    }

                    if (!File.Exists(targetIndicatorFilename))
                    {
                        // copy the placeholder one from the data folder
                        File.Copy(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), PlaceholderIndicatorPath), targetIndicatorFilename);
                    }
                }
                #endregion

                // TODO: make sure that each category is valid, i.e has a correctly named BG, language folder with a font, etc
                for (Int32 gameIndex = 0; gameIndex < category.Entries.Count; ++gameIndex)
                {
                    if (progress != null)
                        progress.Report(++ProcessedCount);

                    Hylo.GameEntry entry = category.Entries[gameIndex];

                    // renumber the entries 
                    entry.ID = gameIndex;

                    // make sure the path is in Unix format
                    entry.Dir = entry.Dir.Replace(@"\", @"/");

                    #region Process Cover Art
                    {
                        // create the cover art folder, and copy files if necessary
                        string targetCoverArtFilename = Path.Combine(Hylo.GetCoverArtPath(targetFolder), FBA.GetGameNameWithPlatform(entry.Platform, entry.GameName), Hylo.CoverArtFilename);

                        // make sure the directory exists
                        string directoryName = Path.GetDirectoryName(targetCoverArtFilename);
                        Directory.CreateDirectory(directoryName);

                        // we only create and copy if we have updated
                        if (!string.IsNullOrEmpty(entry.CustomCoverPath))
                        {
                            if (targetCoverArtFilename != entry.CustomCoverPath)
                            {
                                ExportResizedImageKeepAspectRatio(286, 321, entry.CustomCoverPath, targetCoverArtFilename);
                            }
                        }

                        if (!File.Exists(targetCoverArtFilename))
                        {
                            // copy the placeholder one from the data folder
                            File.Copy(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), PlaceholderCoverArtPath), targetCoverArtFilename);
                        }
                    }
                    #endregion

                    #region Copy fba roms to target
                    // copy the roms from the fba romset to the hylo folder
                    string hyloRomPath = Path.Combine(Hylo.GetRomPath(targetFolder), FBA.GetRomsPrefixFolderFromPlatform(entry.Platform, true));
                    string expectedHyloRomFilename = Path.Combine(hyloRomPath, entry.GameName + @".zip");

                    if (!File.Exists(expectedHyloRomFilename))
                    {
                        // this function handle parent roms 'romof' dependency
                        List<string> fbaRomsetFilename = FbaRomset.GetFbaRomPathFromGameIndex(Settings.FbaRomsFolder, entry.Platform, entry.FbaGameIndex);

                        foreach (string romFilename in fbaRomsetFilename)
                        {
                            string fbaRomZipFilename = Path.GetFileName(romFilename);
                            string targetHyloRomFilename = Path.Combine(hyloRomPath, fbaRomZipFilename);

                            // make sure the directory exists
                            Directory.CreateDirectory(Path.GetDirectoryName(targetHyloRomFilename));

                            // perform copy
                            if (!File.Exists(targetHyloRomFilename))
                            {
                                File.Copy(romFilename, targetHyloRomFilename);
                            }
                        }
                    }
                    #endregion


                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                }

                // save the ini
                HyloGameIniParser.Serialize(category.Entries, Hylo.GetGamesIniFromCategoryIndex(targetFolder, categoryIndex));
            }
        }

        private void AsyncExportHyloToFolder(string targetFolder)
        {
            AsyncProgressDialog frm = new AsyncProgressDialog();

            frm.progressBar1.Minimum = 0;
            frm.progressBar1.Maximum = GetAllGamesToExportCount();

            // The Progress<T> constructor captures our UI context,
            //  so the lambda will be run on the UI thread.
            var progress = new Progress<int>(percent =>
            {
                frm.progressBar1.Value = percent;
            });

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                frm.RunAsync(() => ExportHyloToFolder(targetFolder, progress, cancellationTokenSource.Token), cancellationTokenSource);
                DialogResult result = frm.ShowDialog("Export", "Export in progress, please wait.", "Ok", "Export Complete!");
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = string.Format("Do you want to export to the {0:s} folder?", Settings.HyloXFolder);
            string title = "Export Hylo Pack";
            DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if(result == DialogResult.Cancel)
                return;

            AsyncExportHyloToFolder(Settings.HyloXFolder);
        }

        private void ExportAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if(!string.IsNullOrEmpty(Settings.LastExportAsFolder))
                {
                    fbd.SelectedPath = Settings.LastExportAsFolder;
                }

                DialogResult fbdResult = fbd.ShowDialog();

                if (fbdResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string message = string.Format("Do you want to export to the {0:s} folder?", fbd.SelectedPath);
                    string title = "Export Hylo Pack";
                    DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                        return;

                    if(Settings.LastExportAsFolder != fbd.SelectedPath)
                    {
                        Settings.LastExportAsFolder = fbd.SelectedPath;
                        Settings.SaveSettings();
                    }

                    AsyncExportHyloToFolder(fbd.SelectedPath);
                }
            }
        }

        

        private void coverArtButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            dialog.InitialDirectory = Settings.HyloXFolder;

            if(!string.IsNullOrEmpty(Settings.LastCovertArtFolder))
            {
                dialog.InitialDirectory = Settings.LastCovertArtFolder;
            }

            dialog.Title = "Please select an image for the cover art";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (HylostickGameIniComboBox.SelectedItem != null)
                {
                    Hylo.Category selectedCategory = (Hylo.Category)HylostickGameIniComboBox.SelectedItem;
                    TreeNodeTag theTag = Hylo_treeView2.SelectedNode.Tag as TreeNodeTag;

                    Hylo.GameEntry hyloGame = selectedCategory.Entries[theTag.HyloGameIndex];
                    hyloGame.CustomCoverPath = dialog.FileName;

                    pictureBox1.ImageLocation = dialog.FileName;

                    string customCoverArtPath = Path.GetDirectoryName(dialog.FileName);
                    if(Settings.LastCovertArtFolder != customCoverArtPath)
                    {
                        Settings.LastCovertArtFolder = customCoverArtPath;
                        Settings.SaveSettings();
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            string message = string.Format("{0:s} {1:s}", assemblyName.Name, assemblyName.Version.ToString());
            MessageBox.Show(message, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CategorySettingButton1_Click(object sender, EventArgs e)
        {
            Hylo.Category SelectedCategory = (Hylo.Category)HylostickGameIniComboBox.SelectedItem;
            Int32 SelectedCateoryIndex = HylostickGameIniComboBox.SelectedIndex;

            if(SelectedCategory != null)
            {
                CategoryEditor frm = new CategoryEditor();
                DialogResult result = frm.ShowDialog(string.Format("Edit Category {0}", SelectedCategory.DisplayName), SelectedCategory);

                if (result == DialogResult.OK)
                {
                    // refresh the combo box
                    HylostickGameIniComboBox.DataSource = null;
                    if (HyloHack.Categories.Count > 0)
                    {
                        HylostickGameIniComboBox.DataSource = HyloHack.Categories;
                        HylostickGameIniComboBox.DisplayMember = "DisplayNameWithCount";
                        HylostickGameIniComboBox.SelectedIndex = SelectedCateoryIndex;
                    }
                }
            }
        }

        private void AddCategoryButton_Click(object sender, EventArgs e)
        {
            Hylo.Category newCategory = new Hylo.Category();
            newCategory.FolderName = string.Format("Cat{0:d}", HyloHack.Categories.Count);
            newCategory.DisplayName = "Untitled";
            newCategory.CustomBackgroundPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), PlaceholderBackgroundPath);
            newCategory.CustomIndicatorPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), PlaceholderIndicatorPath);

            CategoryEditor frm = new CategoryEditor();
            DialogResult result = frm.ShowDialog(string.Format("Please enter the new category name"), newCategory);

            if(result == DialogResult.OK)
            {
                // add to the list
                HyloHack.Categories.Add(newCategory);

                // refresh the combo box
                HylostickGameIniComboBox.DataSource = null;
                if (HyloHack.Categories.Count > 0)
                {
                    HylostickGameIniComboBox.DataSource = HyloHack.Categories;
                    HylostickGameIniComboBox.DisplayMember = "DisplayNameWithCount";
                    HylostickGameIniComboBox.SelectedIndex = HyloHack.Categories.Count - 1;
                }
            }
        }

        private void RemoveCategoryButton_Click(object sender, EventArgs e)
        {
            if (HylostickGameIniComboBox.SelectedItem != null)
            {
                Hylo.Category currentCategory = (Hylo.Category)HylostickGameIniComboBox.SelectedItem;

                // we cannot remove the first category, that is a big nono
                if (HylostickGameIniComboBox.SelectedIndex == 0)
                {
                    MessageBox.Show(string.Format("You cannot remove the first category {0:s}", currentCategory.DisplayName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show(string.Format("Do you want to remove the category {0:s}", currentCategory.DisplayName), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    // remove from the list.
                    HyloHack.Categories.Remove(currentCategory);

                    HylostickGameIniComboBox.DataSource = null;
                    if (HyloHack.Categories.Count > 0)
                    {
                        HylostickGameIniComboBox.DataSource = HyloHack.Categories;
                        HylostickGameIniComboBox.DisplayMember = "DisplayNameWithCount";
                        HylostickGameIniComboBox.SelectedIndex = HyloHack.Categories.Count - 1;
                    }
                }
            }
        }

        // got lazy, slightly modified from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public void ExportResizedImageKeepAspectRatio(int expectedWidth, int expectedHeight, string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath))
            {
                return;
            }

            var image = System.Drawing.Image.FromFile(inputPath);

            // if the image is the same size already, then just copy
            if(image.Width == expectedWidth && image.Height == expectedHeight)
            {
                File.Copy(inputPath, outputPath, true);
                image.Dispose();
                return;
            }

            // different size, resize
            double ratioX = (double)expectedWidth / image.Width;
            double ratioY = (double)expectedHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);
            Bitmap newImage = new Bitmap(expectedWidth, expectedHeight);
            Graphics thumbGraph = Graphics.FromImage(newImage);

            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;

            int offsetX = (newWidth - expectedWidth) / 2;
            int offsetY = (newHeight - expectedHeight) / 2;

            thumbGraph.DrawImage(image, -offsetX, -offsetY, newWidth, newHeight);
            image.Dispose();

            newImage.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
