using System;
using System.Collections.Generic;
using System.Linq;

namespace TextExtractor.TestHelpers
{
	public class ADependency
	{
		private ADependency _parent;

		public ADependency Parent
		{
			get { return this._parent; }
		}

		private List<ADependency> _children = new List<ADependency>();

		public IEnumerable<ADependency> Children
		{
			get { return this._children.AsEnumerable(); }
		}

		/// <summary>
		/// Override this method when a parent has settings in a child which may need to be pushed 
		/// </summary>
		public virtual void SharedExecute()
		{
		}

		/// <summary>
		/// Adds a dependency to this object's children 
		/// </summary>
		/// <param name="dependency"></param>
		public void Add(ADependency dependency)
		{
			var child = dependency;
			child._parent = this;
			this._children.Add(child);
		}

		/// <summary>
		/// Removes and orphans the dependency from this object's children 
		/// </summary>
		/// <param name="dependency"></param>
		public void Remove(ADependency dependency)
		{
			var child = dependency;
			child._parent = null;
			this._children.Remove(child);
		}

		/// <summary>
		/// Pulls a child of type T from this object's children. Recurses through subchildren, and begins at tree origin.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Pull<T>() where T : class
		{
			var origin = this.FindOrigin();
			var dependency = origin.InternalPull<T>();

			// If the dependency was found
			if (dependency != default(T)) { return dependency; }

			var message = String.Format("No return dependency of type {0} has been loaded or initialized into any of your ADependency implementations", typeof(T).Name);
			throw new Exception(message);
		}

		/// <summary>
		/// Pulls a child of type T from this object's children. Recurses through subchildren. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected T InternalPull<T>() where T : class
		{
			if (this is T) { return this as T; }

			// Checks this for children and iterates down entire branch 
			if (this.HasChildren(this))
			{
				foreach (var child in this.Children)
				{
					var innerChild = child.InternalPull<T>();
					if (innerChild != default(T)) { return innerChild; }
				}
			}

			// After iterating down branch, begins to traverse back up branch until all
			// possibilities under the top parent have been exhausted
			return default(T);
		}

		/// <summary>
		/// Pushes a change to the Parent by calling its SetMyDependency method 
		/// </summary>
		public void Push()
		{
			this.Parent.SharedExecute();
		}

		#region Cascade - Experimental

		/// <summary>
		/// Calls SharedExecute() on all children, beginning at the origin. 
		/// </summary>
		public void ResetTree()
		{
			var func = new Func<ADependency, Boolean>(
				dep =>
				{
					dep.SharedExecute();
					return true;
				});

			this.Cascade(func);
		}

		/// <summary>
		/// Cascades a custom function that takes an ADependency and returns success to entire tree. 
		/// </summary>
		/// <param name="func"></param>
		protected void Cascade(Func<ADependency, Boolean> func)
		{
			var origin = this.FindOrigin();
			origin.CascadeFunc(func);
		}

		/// <summary>
		/// Returns the top-most parent (origin). Useful to execute a function on the entire tree. 
		/// </summary>
		/// <returns></returns>
		protected ADependency FindOrigin()
		{
			if (this.Parent == null) { return this; }
			return this.Parent.FindOrigin();
		}

		protected void CascadeFunc(Func<ADependency, Boolean> func)
		{
			var success = func.Invoke(this);

			// Checks for children and iterates over all children, running func on each one 
			if (this.HasChildren(this))
			{
				foreach (var child in this.Children)
				{
					child.CascadeFunc(func);
				}
			}
		}

		#endregion Cascade - Experimental

		/// <summary>
		/// Checks this object to see if it has children 
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		protected Boolean HasChildren(ADependency child)
		{
			return (child.Children != null && child.Children.Any());
		}
	}
}