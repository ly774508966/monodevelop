// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.WelcomePage {
    
    
    public partial class WelcomePageFallbackWidget {
        
        private Gtk.Alignment alignment1;
        
        private Gtk.HBox hbox1;
        
        private Gtk.VBox vbox2;
        
        private Gtk.Label headerActions;
        
        private Gtk.HBox hbox3;
        
        private Gtk.Label label1;
        
        private Gtk.VBox actionBox;
        
        private Gtk.Label label4;
        
        private Gtk.Label headerRecentProj;
        
        private Gtk.Label filler2;
        
        private Gtk.HBox hbox4;
        
        private Gtk.Label label5;
        
        private Gtk.VBox vbox3;
        
        private Gtk.Table recentFilesTable;
        
        private Gtk.HSeparator hseparator1;
        
        private Gtk.Label projNameLabel;
        
        private Gtk.Label projTimeLabel;
        
        private Gtk.VBox vbox1;
        
        private Gtk.Label headerSupportLinks;
        
        private Gtk.HBox hbox5;
        
        private Gtk.Label label6;
        
        private Gtk.VBox supportLinkBox;
        
        private Gtk.Label label7;
        
        private Gtk.Label headerDevLinks;
        
        private Gtk.HBox hbox6;
        
        private Gtk.Label label8;
        
        private Gtk.VBox devLinkBox;
        
        private Gtk.Label headerNewsLinks;
        
        private Gtk.HBox hbox7;
        
        private Gtk.Label label9;
        
        private Gtk.VBox newsLinkBox;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.WelcomePage.WelcomePageFallbackWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "MonoDevelop.WelcomePage.WelcomePageFallbackWidget";
            // Container child MonoDevelop.WelcomePage.WelcomePageFallbackWidget.Gtk.Container+ContainerChild
            this.alignment1 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment1.Name = "alignment1";
            // Container child alignment1.Gtk.Container+ContainerChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 35;
            // Container child hbox1.Gtk.Box+BoxChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.headerActions = new Gtk.Label();
            this.headerActions.Name = "headerActions";
            this.headerActions.Xalign = 0F;
            this.headerActions.UseMarkup = true;
            this.vbox2.Add(this.headerActions);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.vbox2[this.headerActions]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox3 = new Gtk.HBox();
            this.hbox3.Name = "hbox3";
            this.hbox3.Spacing = 6;
            // Container child hbox3.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.WidthRequest = 10;
            this.label1.Name = "label1";
            this.hbox3.Add(this.label1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox3[this.label1]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox3.Gtk.Box+BoxChild
            this.actionBox = new Gtk.VBox();
            this.actionBox.Name = "actionBox";
            this.hbox3.Add(this.actionBox);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox3[this.actionBox]));
            w3.Position = 1;
            this.vbox2.Add(this.hbox3);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox3]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.label4 = new Gtk.Label();
            this.label4.WidthRequest = 10;
            this.label4.HeightRequest = 10;
            this.label4.Name = "label4";
            this.vbox2.Add(this.label4);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox2[this.label4]));
            w5.Position = 2;
            w5.Expand = false;
            w5.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.headerRecentProj = new Gtk.Label();
            this.headerRecentProj.WidthRequest = 200;
            this.headerRecentProj.Name = "headerRecentProj";
            this.headerRecentProj.Xalign = 0F;
            this.headerRecentProj.UseMarkup = true;
            this.vbox2.Add(this.headerRecentProj);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.vbox2[this.headerRecentProj]));
            w6.Position = 3;
            w6.Expand = false;
            w6.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.filler2 = new Gtk.Label();
            this.filler2.HeightRequest = 3;
            this.filler2.Name = "filler2";
            this.vbox2.Add(this.filler2);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.vbox2[this.filler2]));
            w7.Position = 4;
            w7.Expand = false;
            w7.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox4 = new Gtk.HBox();
            this.hbox4.Name = "hbox4";
            this.hbox4.Spacing = 6;
            // Container child hbox4.Gtk.Box+BoxChild
            this.label5 = new Gtk.Label();
            this.label5.WidthRequest = 10;
            this.label5.Name = "label5";
            this.hbox4.Add(this.label5);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.hbox4[this.label5]));
            w8.Position = 0;
            w8.Expand = false;
            w8.Fill = false;
            // Container child hbox4.Gtk.Box+BoxChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.recentFilesTable = new Gtk.Table(((uint)(2)), ((uint)(2)), false);
            this.recentFilesTable.Name = "recentFilesTable";
            this.recentFilesTable.RowSpacing = ((uint)(6));
            this.recentFilesTable.ColumnSpacing = ((uint)(4));
            // Container child recentFilesTable.Gtk.Table+TableChild
            this.hseparator1 = new Gtk.HSeparator();
            this.hseparator1.Name = "hseparator1";
            this.recentFilesTable.Add(this.hseparator1);
            Gtk.Table.TableChild w9 = ((Gtk.Table.TableChild)(this.recentFilesTable[this.hseparator1]));
            w9.TopAttach = ((uint)(1));
            w9.BottomAttach = ((uint)(2));
            w9.RightAttach = ((uint)(2));
            w9.YPadding = ((uint)(2));
            w9.XOptions = ((Gtk.AttachOptions)(4));
            w9.YOptions = ((Gtk.AttachOptions)(4));
            // Container child recentFilesTable.Gtk.Table+TableChild
            this.projNameLabel = new Gtk.Label();
            this.projNameLabel.Name = "projNameLabel";
            this.projNameLabel.Xalign = 0F;
            this.recentFilesTable.Add(this.projNameLabel);
            Gtk.Table.TableChild w10 = ((Gtk.Table.TableChild)(this.recentFilesTable[this.projNameLabel]));
            w10.YOptions = ((Gtk.AttachOptions)(4));
            // Container child recentFilesTable.Gtk.Table+TableChild
            this.projTimeLabel = new Gtk.Label();
            this.projTimeLabel.Name = "projTimeLabel";
            this.projTimeLabel.Xalign = 1F;
            this.recentFilesTable.Add(this.projTimeLabel);
            Gtk.Table.TableChild w11 = ((Gtk.Table.TableChild)(this.recentFilesTable[this.projTimeLabel]));
            w11.LeftAttach = ((uint)(1));
            w11.RightAttach = ((uint)(2));
            w11.YOptions = ((Gtk.AttachOptions)(4));
            this.vbox3.Add(this.recentFilesTable);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(this.vbox3[this.recentFilesTable]));
            w12.Position = 0;
            w12.Expand = false;
            w12.Fill = false;
            this.hbox4.Add(this.vbox3);
            Gtk.Box.BoxChild w13 = ((Gtk.Box.BoxChild)(this.hbox4[this.vbox3]));
            w13.Position = 1;
            this.vbox2.Add(this.hbox4);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox4]));
            w14.Position = 5;
            w14.Expand = false;
            w14.Fill = false;
            this.hbox1.Add(this.vbox2);
            Gtk.Box.BoxChild w15 = ((Gtk.Box.BoxChild)(this.hbox1[this.vbox2]));
            w15.Position = 0;
            w15.Expand = false;
            w15.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.headerSupportLinks = new Gtk.Label();
            this.headerSupportLinks.Name = "headerSupportLinks";
            this.headerSupportLinks.Xalign = 0F;
            this.headerSupportLinks.UseMarkup = true;
            this.vbox1.Add(this.headerSupportLinks);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.vbox1[this.headerSupportLinks]));
            w16.Position = 0;
            w16.Expand = false;
            w16.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox5 = new Gtk.HBox();
            this.hbox5.Name = "hbox5";
            this.hbox5.Spacing = 6;
            // Container child hbox5.Gtk.Box+BoxChild
            this.label6 = new Gtk.Label();
            this.label6.WidthRequest = 10;
            this.label6.Name = "label6";
            this.hbox5.Add(this.label6);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.hbox5[this.label6]));
            w17.Position = 0;
            w17.Expand = false;
            w17.Fill = false;
            // Container child hbox5.Gtk.Box+BoxChild
            this.supportLinkBox = new Gtk.VBox();
            this.supportLinkBox.Name = "supportLinkBox";
            this.hbox5.Add(this.supportLinkBox);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(this.hbox5[this.supportLinkBox]));
            w18.Position = 1;
            this.vbox1.Add(this.hbox5);
            Gtk.Box.BoxChild w19 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox5]));
            w19.Position = 1;
            w19.Expand = false;
            w19.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.label7 = new Gtk.Label();
            this.label7.WidthRequest = 10;
            this.label7.HeightRequest = 10;
            this.label7.Name = "label7";
            this.vbox1.Add(this.label7);
            Gtk.Box.BoxChild w20 = ((Gtk.Box.BoxChild)(this.vbox1[this.label7]));
            w20.Position = 2;
            w20.Expand = false;
            w20.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.headerDevLinks = new Gtk.Label();
            this.headerDevLinks.WidthRequest = 200;
            this.headerDevLinks.Name = "headerDevLinks";
            this.headerDevLinks.Xalign = 0F;
            this.headerDevLinks.UseMarkup = true;
            this.vbox1.Add(this.headerDevLinks);
            Gtk.Box.BoxChild w21 = ((Gtk.Box.BoxChild)(this.vbox1[this.headerDevLinks]));
            w21.Position = 3;
            w21.Expand = false;
            w21.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox6 = new Gtk.HBox();
            this.hbox6.Name = "hbox6";
            this.hbox6.Spacing = 6;
            // Container child hbox6.Gtk.Box+BoxChild
            this.label8 = new Gtk.Label();
            this.label8.WidthRequest = 10;
            this.label8.Name = "label8";
            this.hbox6.Add(this.label8);
            Gtk.Box.BoxChild w22 = ((Gtk.Box.BoxChild)(this.hbox6[this.label8]));
            w22.Position = 0;
            w22.Expand = false;
            w22.Fill = false;
            // Container child hbox6.Gtk.Box+BoxChild
            this.devLinkBox = new Gtk.VBox();
            this.devLinkBox.Name = "devLinkBox";
            this.hbox6.Add(this.devLinkBox);
            Gtk.Box.BoxChild w23 = ((Gtk.Box.BoxChild)(this.hbox6[this.devLinkBox]));
            w23.Position = 1;
            this.vbox1.Add(this.hbox6);
            Gtk.Box.BoxChild w24 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox6]));
            w24.Position = 4;
            w24.Expand = false;
            w24.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.headerNewsLinks = new Gtk.Label();
            this.headerNewsLinks.WidthRequest = 200;
            this.headerNewsLinks.Name = "headerNewsLinks";
            this.headerNewsLinks.Xalign = 0F;
            this.headerNewsLinks.UseMarkup = true;
            this.vbox1.Add(this.headerNewsLinks);
            Gtk.Box.BoxChild w25 = ((Gtk.Box.BoxChild)(this.vbox1[this.headerNewsLinks]));
            w25.Position = 5;
            w25.Expand = false;
            w25.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox7 = new Gtk.HBox();
            this.hbox7.Name = "hbox7";
            this.hbox7.Spacing = 6;
            // Container child hbox7.Gtk.Box+BoxChild
            this.label9 = new Gtk.Label();
            this.label9.WidthRequest = 10;
            this.label9.Name = "label9";
            this.hbox7.Add(this.label9);
            Gtk.Box.BoxChild w26 = ((Gtk.Box.BoxChild)(this.hbox7[this.label9]));
            w26.Position = 0;
            w26.Expand = false;
            w26.Fill = false;
            // Container child hbox7.Gtk.Box+BoxChild
            this.newsLinkBox = new Gtk.VBox();
            this.newsLinkBox.Name = "newsLinkBox";
            this.hbox7.Add(this.newsLinkBox);
            Gtk.Box.BoxChild w27 = ((Gtk.Box.BoxChild)(this.hbox7[this.newsLinkBox]));
            w27.Position = 1;
            this.vbox1.Add(this.hbox7);
            Gtk.Box.BoxChild w28 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox7]));
            w28.Position = 6;
            w28.Expand = false;
            w28.Fill = false;
            this.hbox1.Add(this.vbox1);
            Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(this.hbox1[this.vbox1]));
            w29.Position = 1;
            w29.Expand = false;
            w29.Fill = false;
            this.alignment1.Add(this.hbox1);
            this.Add(this.alignment1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
        }
    }
}
