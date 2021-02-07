namespace Cinchoo.Core.Collections
{
	#region NameSpaces

	using System;
	using System.Xml;
	using System.Collections;
	using System.Xml.Serialization;

	using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.Xml.Serialization;

	#endregion NameSpaces

	public class ChoNotNullableArrayList : ArrayList, IXmlSerializable
	{
		#region Instance Data Members (Private)

		private bool _isMixedCollection = false;
		private string _elementType;

		#endregion

		#region Constructors

		public ChoNotNullableArrayList()
		{
		}

		internal ChoNotNullableArrayList(bool trash)
		{
		}

		public ChoNotNullableArrayList(ICollection collection)
		{
			AddRange(collection);
		}

		public ChoNotNullableArrayList(int capacity)
			: base(capacity)
		{
		}

		#endregion

		#region IXmlSerializable Members

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			try
			{
				if (Count > 0)
				{
					if (_isMixedCollection)
					{
						writer.WriteAttributeString("type", "MixedCollection");
						for (int index = 0; index < Count; index++)
						{
							writer.WriteStartElement("Item");
                            writer.WriteAttributeString("type", this[index].GetType().SimpleQualifiedName());
							ChoNullNSXmlSerializer xmlSerializer = new ChoNullNSXmlSerializer(this[index].GetType());
							xmlSerializer.Serialize(writer, this[index]);
							writer.WriteEndElement();
						}
					}
					else
					{
                        writer.WriteAttributeString("type", this[0].GetType().SimpleQualifiedName());
						ChoNullNSXmlSerializer xmlSerializer = new ChoNullNSXmlSerializer(this[0].GetType());
						for (int index = 0; index < Count; index++)
							xmlSerializer.Serialize(writer, this[index]);
					}
				}
			}
			catch (Exception)
			{
				//ChoStreamProfile.WriteLine(ChoReservedFileName.SerializationIssues.ToString(), ChoApplicationException.ToString(ex));
				throw;
			}
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			// TODO:  Add ChoNotNullableArrayList.GetSchema implementation
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			try
			{
				string objectType = reader.GetAttribute("type");
				if (objectType != null && objectType.Trim().Length > 0)
				{
					if (objectType == "MixedCollection")
					{
						int index = 0;
						reader.Read();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if (reader.IsStartElement("Item"))
							{
								objectType = reader.GetAttribute("type");
								Type type = ChoType.GetType(objectType);
								if (type == null) throw new ApplicationException(String.Format("Can't find {0} class.", objectType));

								reader.ReadStartElement("Item");

								try
								{
									index++;
                                    //XmlSerializer xmlSerializer = new XmlSerializer(type);
                                    XmlSerializer xmlSerializer = XmlSerializer.FromTypes(new[] { type }).GetNValue(0);
                                    Add(xmlSerializer.Deserialize(reader));
								}
								catch (Exception ex)
								{
									throw new XmlException(String.Format("Failed to deserialize {0} array item.", index), ex);
								}

								reader.ReadEndElement();
							}
						}
						reader.ReadEndElement();
					}
					else
					{
						Type type = ChoType.GetType(objectType);

						if (type == null) throw new ApplicationException(String.Format("Can't find {0} class.", objectType));

						reader.Read();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							//if (reader.IsStartElement(type.Name))
							//{
                            //XmlSerializer xmlSerializer = new XmlSerializer(ChoType.GetType(objectType));
                            XmlSerializer xmlSerializer = XmlSerializer.FromTypes(new[] { ChoType.GetType(objectType) }).GetNValue(0);
                            Add(xmlSerializer.Deserialize(reader));
							//}
						}
						reader.ReadEndElement();
					}
				}
				else
					reader.Skip();
			}
			catch (Exception)
			{
				//ChoStreamProfile.WriteLine(ChoReservedFileName.SerializationIssues.ToString(), ChoApplicationException.ToString(ex));
				throw;
			}
		}

		#endregion

		#region ChoNotNullableArrayList Overrides

		public static ChoNotNullableArrayList Synchronized(ChoNotNullableArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			return new ChoSynchronizedArrayList(list);
		}

		public static ChoNotNullableArrayList FixedSize(ChoNotNullableArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			return new ChoFixedSizeArrayList(list);
		}

		public static ChoNotNullableArrayList ReadOnly(ChoNotNullableArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			return new ChoReadOnlyArrayList(list);
		}

		public override int Add(object value)
		{
			if (value == null) return -1;

			if (!_isMixedCollection)
			{
				if (_elementType == null)
                    _elementType = value.GetType().SimpleQualifiedName();
				else
				{
                    if (_elementType != value.GetType().SimpleQualifiedName())
						_isMixedCollection = true;
				}
			}

			return base.Add(value);
		}

		public override void AddRange(ICollection c)
		{
			if (c == null) return;

			foreach (object item in c)
				Add(item);
		}

		#endregion

		#region ChoSynchronizedArrayList Class

		[Serializable]
		private class ChoSynchronizedArrayList : ChoNotNullableArrayList
		{
			// Fields
			private ChoNotNullableArrayList _list;
			private object _root;

			// Methods
			internal ChoSynchronizedArrayList(ChoNotNullableArrayList list)
				: base(false)
			{
				this._list = list;
				this._root = list.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (this._root)
				{
					return this._list.Add(value);
				}
			}

			public override void AddRange(ICollection c)
			{
				lock (this._root)
				{
					this._list.AddRange(c);
				}
			}

			public override int BinarySearch(object value)
			{
				lock (this._root)
				{
					return this._list.BinarySearch(value);
				}
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				lock (this._root)
				{
					return this._list.BinarySearch(value, comparer);
				}
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				lock (this._root)
				{
					return this._list.BinarySearch(index, count, value, comparer);
				}
			}

			public override void Clear()
			{
				lock (this._root)
				{
					this._list.Clear();
				}
			}

			public override object Clone()
			{
				lock (this._root)
				{
					return new ChoNotNullableArrayList.ChoSynchronizedArrayList((ChoNotNullableArrayList)this._list.Clone());
				}
			}

			public override bool Contains(object item)
			{
				lock (this._root)
				{
					return this._list.Contains(item);
				}
			}

			public override void CopyTo(Array array)
			{
				lock (this._root)
				{
					this._list.CopyTo(array);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (this._root)
				{
					this._list.CopyTo(array, index);
				}
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				lock (this._root)
				{
					this._list.CopyTo(index, array, arrayIndex, count);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (this._root)
				{
					return this._list.GetEnumerator();
				}
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				lock (this._root)
				{
					return this._list.GetEnumerator(index, count);
				}
			}

			public new ChoNotNullableArrayList GetRange(int index, int count)
			{
				lock (this._root)
				{
					return new ChoNotNullableArrayList(this._list.GetRange(index, count));
				}
			}

			public override int IndexOf(object value)
			{
				lock (this._root)
				{
					return this._list.IndexOf(value);
				}
			}

			public override int IndexOf(object value, int startIndex)
			{
				lock (this._root)
				{
					return this._list.IndexOf(value, startIndex);
				}
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				lock (this._root)
				{
					return this._list.IndexOf(value, startIndex, count);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (this._root)
				{
					this._list.Insert(index, value);
				}
			}

			public override void InsertRange(int index, ICollection c)
			{
				lock (this._root)
				{
					this._list.InsertRange(index, c);
				}
			}

			public override int LastIndexOf(object value)
			{
				lock (this._root)
				{
					return this._list.LastIndexOf(value);
				}
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				lock (this._root)
				{
					return this._list.LastIndexOf(value, startIndex);
				}
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				lock (this._root)
				{
					return this._list.LastIndexOf(value, startIndex, count);
				}
			}

			public override void Remove(object value)
			{
				lock (this._root)
				{
					this._list.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (this._root)
				{
					this._list.RemoveAt(index);
				}
			}

			public override void RemoveRange(int index, int count)
			{
				lock (this._root)
				{
					this._list.RemoveRange(index, count);
				}
			}

			public override void Reverse(int index, int count)
			{
				lock (this._root)
				{
					this._list.Reverse(index, count);
				}
			}

			public override void SetRange(int index, ICollection c)
			{
				lock (this._root)
				{
					this._list.SetRange(index, c);
				}
			}

			public override void Sort()
			{
				lock (this._root)
				{
					this._list.Sort();
				}
			}

			public override void Sort(IComparer comparer)
			{
				lock (this._root)
				{
					this._list.Sort(comparer);
				}
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				lock (this._root)
				{
					this._list.Sort(index, count, comparer);
				}
			}

			public override object[] ToArray()
			{
				lock (this._root)
				{
					return this._list.ToArray();
				}
			}

			public override Array ToArray(Type type)
			{
				lock (this._root)
				{
					return this._list.ToArray(type);
				}
			}

			public override void TrimToSize()
			{
				lock (this._root)
				{
					this._list.TrimToSize();
				}
			}

			// Properties
			public override int Capacity
			{
				get
				{
					lock (this._root)
					{
						return this._list.Capacity;
					}
				}
				set
				{
					lock (this._root)
					{
						this._list.Capacity = value;
					}
				}
			}

			public override int Count
			{
				get
				{
					lock (this._root)
					{
						return this._list.Count;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return this._list.IsFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this._list.IsReadOnly;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object this[int index]
			{
				get
				{
					lock (this._root)
					{
						return this._list[index];
					}
				}
				set
				{
					lock (this._root)
					{
						this._list[index] = value;
					}
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this._root;
				}
			}
		}

		#endregion ChoSynchronizedArrayList Class

		#region ChoFixedSizeArrayList Class

		[Serializable]
		private class ChoFixedSizeArrayList : ChoNotNullableArrayList
		{
			// Fields
			private ChoNotNullableArrayList _list;

			// Methods
			internal ChoFixedSizeArrayList(ChoNotNullableArrayList l)
			{
				this._list = l;
			}

			public override int Add(object obj)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				return this._list.BinarySearch(index, count, value, comparer);
			}

			public override void Clear()
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override object Clone()
			{
				ChoNotNullableArrayList.ChoFixedSizeArrayList list = new ChoNotNullableArrayList.ChoFixedSizeArrayList(this._list);
				list._list = (ChoNotNullableArrayList)this._list.Clone();
				return list;
			}

			public override bool Contains(object obj)
			{
				return this._list.Contains(obj);
			}

			public override void CopyTo(Array array, int index)
			{
				this._list.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				this._list.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return this._list.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				return this._list.GetEnumerator(index, count);
			}

			public new ChoNotNullableArrayList GetRange(int index, int count)
			{
				if ((index < 0) || (count < 0))
				{
					throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
				}
				if ((this.Count - index) < count)
				{
					throw new ArgumentException("Argument_InvalidOffLen");
				}
				return new ChoNotNullableArrayList(ChoArray.SubArray(this, index, count));
			}

			public override int IndexOf(object value)
			{
				return this._list.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return this._list.IndexOf(value, startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				return this._list.IndexOf(value, startIndex, count);
			}

			public override void Insert(int index, object obj)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override int LastIndexOf(object value)
			{
				return this._list.LastIndexOf(value);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return this._list.LastIndexOf(value, startIndex);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				return this._list.LastIndexOf(value, startIndex, count);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			public override void Reverse(int index, int count)
			{
				this._list.Reverse(index, count);
			}

			public override void SetRange(int index, ICollection c)
			{
				this._list.SetRange(index, c);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				this._list.Sort(index, count, comparer);
			}

			public override object[] ToArray()
			{
				return this._list.ToArray();
			}

			public override Array ToArray(Type type)
			{
				return this._list.ToArray(type);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException("NotSupported_FixedSizeCollection");
			}

			// Properties
			public override int Capacity
			{
				get
				{
					return this._list.Capacity;
				}
				set
				{
					throw new NotSupportedException("NotSupported_FixedSizeCollection");
				}
			}

			public override int Count
			{
				get
				{
					return this._list.Count;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this._list.IsReadOnly;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return this._list.IsSynchronized;
				}
			}

			public override object this[int index]
			{
				get
				{
					return this._list[index];
				}
				set
				{
					this._list[index] = value;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this._list.SyncRoot;
				}
			}
		}

		#endregion ChoFixedSizeArrayList Class

		#region ChoReadOnlyArrayList Class

		[Serializable]
		private class ChoReadOnlyArrayList : ChoNotNullableArrayList
		{
			// Fields
			private ChoNotNullableArrayList _list;

			// Methods
			internal ChoReadOnlyArrayList(ChoNotNullableArrayList l)
			{
				this._list = l;
			}

			public override int Add(object obj)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				return this._list.BinarySearch(index, count, value, comparer);
			}

			public override void Clear()
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override object Clone()
			{
				ChoNotNullableArrayList.ChoReadOnlyArrayList list = new ChoNotNullableArrayList.ChoReadOnlyArrayList(this._list);
				list._list = (ChoNotNullableArrayList)this._list.Clone();
				return list;
			}

			public override bool Contains(object obj)
			{
				return this._list.Contains(obj);
			}

			public override void CopyTo(Array array, int index)
			{
				this._list.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				this._list.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return this._list.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				return this._list.GetEnumerator(index, count);
			}

			public new ChoNotNullableArrayList GetRange(int index, int count)
			{
				if ((index < 0) || (count < 0))
				{
					throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
				}
				if ((this.Count - index) < count)
				{
					throw new ArgumentException("Argument_InvalidOffLen");
				}
				return new ChoNotNullableArrayList(ChoArray.SubArray(this, index, count));
			}

			public override int IndexOf(object value)
			{
				return this._list.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return this._list.IndexOf(value, startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				return this._list.IndexOf(value, startIndex, count);
			}

			public override void Insert(int index, object obj)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override int LastIndexOf(object value)
			{
				return this._list.LastIndexOf(value);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return this._list.LastIndexOf(value, startIndex);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				return this._list.LastIndexOf(value, startIndex, count);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void SetRange(int index, ICollection c)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			public override object[] ToArray()
			{
				return this._list.ToArray();
			}

			public override Array ToArray(Type type)
			{
				return this._list.ToArray(type);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException("NotSupported_ReadOnlyCollection");
			}

			// Properties
			public override int Capacity
			{
				get
				{
					return this._list.Capacity;
				}
				set
				{
					throw new NotSupportedException("NotSupported_ReadOnlyCollection");
				}
			}

			public override int Count
			{
				get
				{
					return this._list.Count;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return this._list.IsSynchronized;
				}
			}

			public override object this[int index]
			{
				get
				{
					return this._list[index];
				}
				set
				{
					throw new NotSupportedException("NotSupported_ReadOnlyCollection");
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this._list.SyncRoot;
				}
			}
		}

		#endregion ChoReadOnlyArrayList Class
	}
}
