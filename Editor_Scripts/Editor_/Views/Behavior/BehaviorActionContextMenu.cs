using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using UnityEngine;

using SpaceAR.Core.Behavior;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System;

namespace SpaceAR.Editor.Behavior
{
    public static class BehaviorActionContextMenu
    {                
        public static void OpenBehaviorContextMenu(System.Action<BehaviorComponent> action)
        {
            IContextMenu contextMenu = IOC.Resolve<IContextMenu>();
            
            IRTE rte = IOC.Resolve<IRTE>();
            GameObject selectedGameObject = rte.Selection.activeGameObject;

            Type sourceType = typeof(SpaceAR.Core.Behavior.Trigger);
            List<Type> typeList = Assembly.GetAssembly(sourceType).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(sourceType)).ToList();

            List<MenuItemInfo> menuItemInfoList = new List<MenuItemInfo>();
            foreach (Type type in typeList)
            {
                MenuItemInfo menuItem = new MenuItemInfo();

                MethodInfo methodGetActionPath = type.GetMethod("GetTriggerPath");
                if (methodGetActionPath != null)
                {
                    string path = (string)methodGetActionPath.Invoke(null, null);
                    menuItem.Path = path;
                }
                else
                {
                    menuItem.Path = type.Name;
                }

                menuItem.Validate = new MenuItemValidationEvent();
                menuItem.Action = new MenuItemEvent();

                MethodInfo methodValidate = type.GetMethod("Validate");
                menuItem.Validate.AddListener((args) => {
                    if (selectedGameObject == null)
                    {
                        args.IsValid = false;
                    }
                    else
                    {
                        if (methodValidate != null)
                        {
                            args.IsValid = (bool)methodValidate.Invoke(null, new object[] { selectedGameObject });
                        }
                        else
                        {
                            if (methodValidate != null)
                            {
                                args.IsValid = (bool)methodValidate.Invoke(null, new object[] { selectedGameObject });
                            }
                            else
                            {
                                args.IsValid = true;
                            }
                        }
                    }
                });

                menuItem.Action.AddListener((args) =>
                {
                    BehaviorComponent newBehaviorComponent = selectedGameObject.AddComponent<BehaviorComponent>();
                    newBehaviorComponent.Trigger = (Core.Behavior.Trigger)Activator.CreateInstance(type);
                    action(newBehaviorComponent);
                });

                menuItemInfoList.Add(menuItem);
            }

            contextMenu.Open(menuItemInfoList.ToArray());
        }

        
        public static void OpenActionNodeContextMenu(System.Action<SpaceAR.Core.Behavior.Action> action)
        {
            IContextMenu contextMenu = IOC.Resolve<IContextMenu>();

            IRTE rte = IOC.Resolve<IRTE>();
            GameObject selectedGameObject = rte.Selection.activeGameObject;
            
            Type sourceType = typeof(SpaceAR.Core.Behavior.Action);
            List<Type> typeList = Assembly.GetAssembly(sourceType).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(sourceType)).ToList();

            List<MenuItemInfo> menuItemInfoList = new List<MenuItemInfo>();
            foreach (Type type in typeList)
            {
                MenuItemInfo menuItem = new MenuItemInfo();
                
                MethodInfo method = type.GetMethod("GetActionPath");
                if (method != null)
                {
                    string path = (string)method.Invoke(null, null);
                    menuItem.Path = path;
                }
                else
                {
                    menuItem.Path = type.Name;
                }

                menuItem.Validate = new MenuItemValidationEvent();
                menuItem.Action = new MenuItemEvent();

                MethodInfo methodValidate = type.GetMethod("Validate");
                menuItem.Validate.AddListener((args) => {
                    if (selectedGameObject == null)
                    {
                        args.IsValid = false;
                    }
                    else
                    {
                        if (methodValidate != null)
                        {
                            args.IsValid = (bool)methodValidate.Invoke(null, new object[] { selectedGameObject });
                        }
                        else
                        {
                            args.IsValid = true;
                        }
                    }
                });

                menuItem.Action.AddListener((args) =>
                {
                    action((Core.Behavior.Action)Activator.CreateInstance(type));
                });

                menuItemInfoList.Add(menuItem);
            }

            contextMenu.Open(menuItemInfoList.ToArray());
        }
        
        static void ValidateBehaviorMenu(MenuItemValidationArgs args)
        {
            IRTE rte = IOC.Resolve<IRTE>();
            GameObject selectedGameObject = rte.Selection.activeGameObject;
            
            if (selectedGameObject == null)
            {
                args.IsValid = false;
            }    
            else
            {
                args.IsValid = true;
            }
        }
    }
}
