using System;
using System.Collections;
using System.Collections.Generic;

namespace SerializationLab
{
    public class ConsoleEditor
    {
        private class ActionInfo
        {
            public string Description { get; }

            public Action Action { get; }


            public ActionInfo(Action action, string description)
            {
                Action = action;
                Description = description;
            }
        }


        private bool _isActive;

        private IPropertyHandler _propertyHandler;

        private ISerializer _serializer;

        private IFilesHandler _filesHandler;

        private List<object> _editingObjects;

        private Dictionary<int, ActionInfo> _actions;

        private const string ObjectStr = "OBJECT";

        private const string EnumerableString = "ENUMERABLE";

        private const string PrimitiveStr = "PRIMITIVE";

        private const string ListStr = "LIST";


        public ConsoleEditor(IPropertyHandler propertyHandler, ISerializer serializer, IFilesHandler filesHandler, object editingObject)
        {
            _isActive = false;
            _propertyHandler = propertyHandler;
            _serializer = serializer;
            _filesHandler = filesHandler;

            _editingObjects = new List<object> { editingObject };
            InitActions();
        }


        public void Start()
        {
            _isActive = true;

            while (_isActive)
            {
                Console.Clear();
                InvokeAction();

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void PrintEditingObject()
        {
            object obj = _editingObjects[_editingObjects.Count - 1];
            PrintObject(obj);
        }

        private void PrintObject(object obj)
        {
            if (obj is IList)
            {
                PrintObjectCollection(obj as IList);
                return;
            }

            if (!(obj is IEnumerable) && !(obj.GetType().IsPrimitive))
            {
                PrintObjectProperties(obj);
                return;
            }

            throw new Exception("Invalid type");
        }

        private void PrintObjectCollection(IList list)
        {
            if (list.Count > 0)
            {
                Console.WriteLine("Editable collection: ");
                int index = 1;
                foreach (var item in list)
                {
                    if (item is IEnumerable)
                    {
                        if (item is IList)
                            Console.WriteLine("  {0}. [{1}] - {2}", index, item.GetType().ToString(), ListStr);
                        else
                            Console.WriteLine("  {0}. [{1}] - {2}", index, item.GetType().ToString(), EnumerableString);
                    }
                    else if (item.GetType().IsPrimitive)
                        Console.WriteLine("  {0}. [{1}] - {2}", index, item.GetType().ToString(), PrimitiveStr);
                    else
                        Console.WriteLine("  {0}. [{1}] - {2}", index, item.GetType().ToString(), ObjectStr);

                    ++index;
                }
            }
            else
                Console.WriteLine("This collection is empty");
            Console.WriteLine();
        }

        private void PrintObjectProperties(object obj)
        {
            List<ExtendedPropertyInfo> list = _propertyHandler.GetProperties(obj);

            if (list.Count > 0)
            {
                Console.WriteLine("Object properties: ");
                int index = 1;
                foreach (var item in list)
                {
                    if (item.Property.PropertyType.IsPrimitive)
                        Console.WriteLine("  {0}. [{1}] - \"{2}\" = {3}", index, item.Property.PropertyType.ToString(), item.Property.Name, item.PropertyValue);
                    else if (item.Property.PropertyType is IEnumerable)
                    {
                        if (item is IList)
                            Console.WriteLine("  {0}. [{1}] - \"{2}\" - {3}", index, item.Property.PropertyType.ToString(), item.Property.Name, ListStr);
                        else
                            Console.WriteLine("  {0}. [{1}] - \"{2}\" - {3}", index, item.Property.PropertyType.ToString(), item.Property.Name, EnumerableString);
                    }
                    else
                        Console.WriteLine("  {0}. [{1}] - \"{2}\" - {3}", index, item.Property.PropertyType.ToString(), item.Property.Name, ObjectStr);

                    ++index;
                }
            }
            else
                Console.WriteLine("Object has no properties to edit");
            Console.WriteLine();
        }

        private int EnterCollectionIndex(IList list)
        {
            while (true)
            {
                Console.Write("Enter index: ");
                if ((int.TryParse(Console.ReadLine(), out int res)) && (res > 0) && (res <= list.Count))
                    return res - 1;
                else
                    Console.WriteLine("Incorrect index");
            }
        }

        private void MoveIn()
        {
            object obj = _editingObjects[_editingObjects.Count - 1];
            Type type = obj.GetType();

            PrintObject(obj);

            if (obj is IList)
            {
                int index = EnterCollectionIndex(obj as IList);
                _editingObjects.Add((obj as IList)[index]);
            }
            else if ((!(obj is IEnumerable)) && (!type.IsPrimitive))
            {
                var props = _propertyHandler.GetProperties(obj);
                int index = EnterCollectionIndex(props);
                var propInfo = props[index];

                if (!propInfo.Property.PropertyType.IsPrimitive)
                    _editingObjects.Add(propInfo.PropertyValue);
                else
                    Console.WriteLine("Cannot \"unwrap\" this type: {0}", propInfo.Property.PropertyType.ToString());
            }
            else
                Console.WriteLine("Cannot \"unwrap\" this type: {0}", type.ToString());
        }

        private void MoveOut()
        {
            if (_editingObjects.Count > 1)
                _editingObjects.RemoveAt(_editingObjects.Count - 1);
            else
                Console.WriteLine("Cannot remove outermost object");
        }

        private void Exit() => _isActive = false;

        private void InvokeAction()
        {
            PrintEditingObject();
            Console.WriteLine();

            Console.WriteLine("Choose an action:");
            foreach (var item in _actions)
                Console.WriteLine("  Press {0} to {1}", item.Key, item.Value.Description);
            Console.WriteLine();

            while (true)
            {
                Console.Write("Enter action key: ");
                string keyStr = Console.ReadLine();

                int key;
                if (!int.TryParse(keyStr, out key))
                    Console.WriteLine("Invalid input");
                else
                {
                    foreach (var item in _actions)
                    {
                        if (item.Key == key)
                        {
                            Console.Clear();
                            item.Value.Action.Invoke();
                            return;
                        }
                    }

                    Console.WriteLine("Action not found");
                }
            }
        }

        private void AddItemToCollection()
        {
            if (_editingObjects[_editingObjects.Count - 1] is IList)
            {
                PrintEditingObject();

                Console.Write("Enter the type name: ");
                object obj = _propertyHandler.CreateObject(Console.ReadLine());

                if (obj != null)
                {
                    try
                    {
                        IList list = (IList)_editingObjects[_editingObjects.Count - 1];
                        list.Add(obj);
                        Console.WriteLine("Success");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Cannot add object to the collection");
                    }
                }
                else
                    Console.WriteLine("Cannot create object");
            }
            else
                Console.WriteLine("This operation cannot be executed");
        }

        private void RemoveItemFromCollection()
        {
            if ((_editingObjects[_editingObjects.Count - 1] is IList))
            {
                IList list = _editingObjects[_editingObjects.Count - 1] as IList;
                if (list.Count > 0)
                {

                    PrintEditingObject();

                    int index = EnterCollectionIndex(list);
                    list.RemoveAt(index);
                    Console.WriteLine("Item have been deleted");
                }
                else
                    Console.WriteLine("This collection is empty");
            }
            else
                Console.WriteLine("This operation cannot be executed");
        }

        private void EditObjectProperty()
        {
            object obj = _editingObjects[_editingObjects.Count - 1];
            if (!(obj is IEnumerable) && (!obj.GetType().IsPrimitive))
            {
                PrintEditingObject();
                int index = EnterCollectionIndex(_propertyHandler.GetProperties(obj));
                var prop = _propertyHandler.GetProperty(obj, index);

                if (prop != null)
                {
                    Console.WriteLine("Current value: {0}", prop.GetValue(obj).ToString());
                    Console.Write("Enter property value: ");
                    string value = Console.ReadLine();

                    if (_propertyHandler.TrySetProperty(obj, value, prop))
                        Console.WriteLine("Success");
                    else
                        Console.WriteLine("Cannot set property value");
                }
                else
                    Console.WriteLine("Property not found");
            }
            else
                Console.WriteLine("This operation cannot be executed");
        }

        private void SerializeAll()
        {
            try
            {
                PrintJsonFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot read directory: {0}", e.Message);
                return;
            }

            Console.Write("Enter file name: ");
            string name = Console.ReadLine();

            try
            {
                string data = _serializer.Serialize(_editingObjects[0]);
                _filesHandler.Rewrite(data, name);
            }
            catch(Exception e)
            {
                Console.WriteLine("Serialization error: {0}", e.Message);
                return;
            }

            Console.WriteLine("Success");
        }

        private void DeserializeAll()
        {
            try
            {
                PrintJsonFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot read directory: {0}", e.Message);
                return;
            }

            Console.Write("Enter file name: ");
            string name = Console.ReadLine();
            Console.Write("Enter type of the target object");
            string typeName = Console.ReadLine();
            Type type = Type.GetType(typeName);

            if (type == null)
            {
                Console.WriteLine("Cannot determine the type");
                return;
            }

            try
            {
                string data = _filesHandler.Read(name);
                var res = _serializer.Deserialize(data, type);
                _editingObjects.Clear();
                _editingObjects.Add(res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deserialization error: {0}", e.Message);
                return;
            }

            Console.WriteLine("Success");
        }

        private void PrintJsonFiles()
        {
            Console.WriteLine("JSON files in directory:");

            var list = _filesHandler.GetIdentifiers();
            int i = 1;
            foreach (var file in list)
            {
                Console.WriteLine("{0}. {1}", i, file);
                ++i;
            }

            Console.WriteLine();
        }

        private void InitActions()
        {
            _actions = new Dictionary<int, ActionInfo>
            {
                { 1, new ActionInfo(MoveIn, "to move in") },
                { 2, new ActionInfo(MoveOut, "to move out") },
                { 3, new ActionInfo(EditObjectProperty, "to edit object properties") },
                { 4, new ActionInfo(AddItemToCollection, "to add item to collection") },
                { 5, new ActionInfo(RemoveItemFromCollection, "to remove item from collection") },
                { 6, new ActionInfo(SerializeAll, "to serialize all") },
                { 7, new ActionInfo(DeserializeAll, "to deserialize all") },
                { 8, new ActionInfo(Exit, "to exit") }
            };
        }
    }
}
