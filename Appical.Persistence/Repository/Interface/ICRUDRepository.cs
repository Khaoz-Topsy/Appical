using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appical.Persistence.Repository.Interface
{
    /// <summary>
    /// The interface a database repository must follow
    /// </summary>
    /// <typeparam name="T">The persistence Entity that is being CRUD</typeparam>
    public interface ICrudRepository<T>
    {
        /// <summary>
        /// Create <see cref="T" /> in table
        /// </summary>
        /// <param name="dto"></param>
        Task<T> Create(T dto);

        /// <summary>
        /// Return all existing <see cref="T" />
        /// </summary>
        Task<List<T>> Read();

        /// <summary>
        /// Fetch <see cref="T" /> existing in the table that matches the <param name="id"></param> specified
        /// </summary>
        /// <param name="id"></param>
        Task<T> Read(Guid id);

        /// <summary>
        /// Update existing <see cref="T" /> that matches the id within the <param name="dto"></param> supplied
        /// </summary>
        /// <param name="dto"></param>
        Task<T> Update(T dto);

        /// <summary>
        /// Delete existing <see cref="T" /> that matches the <param name="id"></param> specified
        /// </summary>
        /// <param name="id"></param>
        Task<T> Delete(Guid id);
    }
}
