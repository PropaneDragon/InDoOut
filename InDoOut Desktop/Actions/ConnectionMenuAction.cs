using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Desktop.Actions
{
    internal class ConnectionMenuAction : Action
    {
        private readonly IUIConnection _connection;
        private readonly IBlockView _blockView;

        public ConnectionMenuAction(IUIConnection connection, IBlockView blockView, Point mousePosition) : base()
        {
            _connection = connection;
            _blockView = blockView;

            if (_connection != null && _blockView != null)
            {
                var rightClickMenu = new ContextMenu()
                {
                    Placement = PlacementMode.MousePoint,
                    IsOpen = true
                };

                rightClickMenu.Closed += RightClickMenu_Closed;

                var deleteItem = new MenuItem() { Header = "Delete" };
                deleteItem.Click += DeleteItem_Click;

                _ = rightClickMenu.Items.Add(deleteItem);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (_blockView != null && _connection != null && _connection.AssociatedStart is IUIOutput output && _connection.AssociatedEnd is IUIInput input)
            {
                if (output.AssociatedOutput.Disconnect(input.AssociatedInput))
                {
                    _blockView?.DeletableRemover?.Remove(_connection);
                }
                else
                {
                    _ = MessageBox.Show("Couldn't delete the connection due to an error.");
                }
            }
        }

        private void RightClickMenu_Closed(object sender, RoutedEventArgs e)
        {
            Finish(null);
        }
    }
}