using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GridIOInterface {
    public class EntityBarManager {
        private ECSFramework framework => ECSFramework.instance;
        private MainWindow window;
        private StackPanel panel;
        private Dictionary<Entity, Label> entityLabels;

        public EntityBarManager(MainWindow window, StackPanel panel) {
            this.window = window;
            this.panel = panel;
            this.entityLabels = new Dictionary<Entity, Label>();

            /*Entity mainCamera = framework.CreateEntity("Main Camera");
            Entity kockle = framework.CreateEntity("Kockle");

            mainCamera.transform.position = new Vector3(0, 0, -5);
            framework.AddComponent(new Camera(), mainCamera);
            framework.AddComponent(new Texture("kockle.jpg"), kockle);*/

            foreach (Entity entity in framework.GetEntities()) {
                AddEntityLabel(entity);
            }
        }

        public void AddEntityLabel(Entity entity) {
            Label entityLabel = new Label {
                Name = "E" + entity.id.ToString(),
            };
            entityLabel.MouseEnter += Label_MouseEnter;
            entityLabel.MouseLeave += Label_MouseLeave;
            entityLabel.MouseLeftButtonDown += Label_MouseLeftButtonDown;
            entityLabels.Add(entity, entityLabel);
            RefreshEntityName(entity);
            panel.Children.Add(entityLabel);
        }

        public void RefreshEntityName(Entity entity) {
            entityLabels[entity].Content = framework.GetComponentOfEntity<EntityInfo>(entity).name;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e) {
            Label label = (Label)sender;
            label.Background = Brushes.GhostWhite;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e) {
            Label label = (Label)sender;
            label.Background = Brushes.Transparent;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Label label = (Label)sender;
            ComponentBarManager componentBarManager = window.componentBarManager;
            componentBarManager.SetEntity(ECSFramework.instance.GetEntities()[(Entity)int.Parse(label.Name.Substring(1))]);
        }
    }
}