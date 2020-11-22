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
using System.Data.Entity;
using AutoLotModel;
using System.Data;

namespace Hutanu_Diana_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        Binding firstNameBinding = new Binding();
        Binding lastNameBinding = new Binding();
        Binding colorBinding = new Binding();
        Binding makeBinding = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            firstNameBinding.Path = new PropertyPath("FirstName");
            lastNameBinding.Path = new PropertyPath("LastName");
            colorBinding.Path = new PropertyPath("Color");
            makeBinding.Path = new PropertyPath("Make");

            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, colorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeBinding);


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));

            //customerOrdersViewSource.Source = ctx.Orders.Local;
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;

            ctx.Customers.Load();
            ctx.Orders.Load();
            ctx.Inventories.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]
            BindDataGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
            }

            // AICI SE PUNE??
            SetValidationBinding();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            SetValidationBinding();

            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            customerDataGrid.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;

            customerDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;

                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;

                ordersDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }

            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                BindDataGrid();
                //customerOrdersViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(selectedOrder);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                ordersDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
                colorTextBox.SetBinding(TextBox.TextProperty, colorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, makeBinding);
            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                ordersDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
                colorTextBox.SetBinding(TextBox.TextProperty, colorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, makeBinding);
            }
    }
        private void btnOrdNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;

            ordersDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;

            ordersDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;

            ordersDataGrid.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnDelete1.IsEnabled = true;

            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;

            ordersDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, colorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeBinding);
        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValid());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }
    }
}
