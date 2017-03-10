using Microsoft.EnterpriseManagement.ConsoleFramework.Wpf;
using Microsoft.EnterpriseManagement.GenericForm;
using Microsoft.EnterpriseManagement.ServiceManager.Applications.ProblemManagement.Forms;
using Microsoft.EnterpriseManagement.UI.Extensions.Shared;
using Microsoft.EnterpriseManagement.UI.WpfControls;
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
using Microsoft.EnterpriseManagement.UI.DataModel;

namespace Custom.SM.PR.UserControlOverride
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Override : UserControl
    {
        private bool flag = false;
        public Override()
        {
            InitializeComponent();
        }
        private ListPicker lp = null;
        private Binding myBinding = null;

		// this is not my function.
		// It is a common function found on the Technet forums that will recursively search a SCSM form for a certain control-type
        private DependencyObject GetParentDependancyObject(DependencyObject child, string name)
        {
            try
            {
                //We need the logical tree to get our parent
                DependencyObject parent = LogicalTreeHelper.GetParent(child);
                DependencyObject lastparent = null;
                //Is the parent our specified control?
                if (name != "" && parent.GetType().ToString() == name) return parent;
                //No, process further
                while (parent != null)
                {
                    string s = parent.GetType().ToString();
                    if (s == name && name != "") return parent;
                    parent = LogicalTreeHelper.GetParent(parent);
                    if (parent != null) lastparent = parent;
                }
                //Return results
                if (name != "") return null;
                else return lastparent;
            }
            catch
            {
                return null;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Because a form can have its data-context (= workitem data that is linked to it) changed multiple times, we set a flag after the first execution to prevent our code from firing more than once.
            if (flag == false)
            {
				// Find the tab-control of our form (it is important when adding our custom usercontrol in the MP XML to use a parent that is in the general tabcontrol)
                DependencyObject doTabControl = GetParentDependancyObject(this,
                    "Microsoft.EnterpriseManagement.ServiceManager.Applications.ProblemManagement.Forms.GeneralTabControl");

                GeneralTabControl tabitem_general = (GeneralTabControl) doTabControl;
				// Get the existing assigned-to field, and get its parent container, which is a stackpanel. A stackpanel adds controls right after another, so is ideal for adding extra controls without worrying too much about layout.
                UserPicker userPicker = (UserPicker) tabitem_general.FindName("AssignedToUserPicker");
                System.Windows.Controls.StackPanel parent = (System.Windows.Controls.StackPanel) userPicker.Parent;
				// Define the extra controls and bind them to the data
                lp = new ListPicker(new Guid("e24ffbc7-78fd-a2da-e4b1-32312a8035ce"));
                myBinding = new Binding();
                myBinding.Source = this.DataContext as IDataItem;
                myBinding.Path = new PropertyPath("SupportGroup");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lp.Name = "SupportGroupListPicker";
                lp.Width = Double.NaN; // AUTO
                lp.Height = Double.NaN; // AUTO
                lp.HorizontalAlignment = HorizontalAlignment.Stretch;
                lp.VerticalAlignment = VerticalAlignment.Center;
                Label lbl = new Label();
                lbl.HorizontalAlignment = HorizontalAlignment.Left;
                lbl.VerticalAlignment = VerticalAlignment.Center;
                lbl.Content = "Support Group:";
				// Add the controls to the stackpanel
                parent.Children.Add(lbl);
                parent.Children.Add(lp);

                flag = true;

            }
			// Update the binding when data is changed
            else
            {
                
                myBinding = new Binding();
                myBinding.Source = this.DataContext as IDataItem;
                myBinding.Path = new PropertyPath("SupportGroup");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lp.SetBinding(ListPicker.SelectedItemProperty, myBinding);

            }
            





        }
    }
}
