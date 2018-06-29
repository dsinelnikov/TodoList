using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Core.Exceptions;
using TodoList.InMemoryStorage;
using Xunit;

namespace InMemoryStorage.Tests
{
    public class EntityStorageTests
    {
        private readonly EntityStorage<IEntity<int>, int> _storage;
        private readonly ReaderWriterLockSlim _accessLock;
        private readonly IDictionary<int, IEntity<int>> _internalStorageItems;

        public EntityStorageTests()
        {
            _internalStorageItems = new Dictionary<int, IEntity<int>>();
            _accessLock = new ReaderWriterLockSlim();
            _storage = new EntityStorage<IEntity<int>, int>(_internalStorageItems, _accessLock, TimeSpan.FromMilliseconds(50));
        }

        [Fact]        
        public async Task Add_NullEntity()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _storage.Add(null));
        }

        [Fact]
        public async Task Add_ExistsItem()
        {
            // Arrange
            var item = new MockEntity(1);
            await _storage.Add(item);

            // Act & Assert
            await Assert.ThrowsAsync<ItemExistsException>(async () => await _storage.Add(item));
        }

        [Fact]
        public async Task Add_ReadIsLocked_LongerThenTimeout()
        {
            // Arrange
            var item = new MockEntity(1);

            using (var assertsCheckedEvent = new AutoResetEvent(false))
            {
                // Act
                using (var lockThreadStartedEvent = new AutoResetEvent(false))
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        _accessLock.EnterReadLock();
                        lockThreadStartedEvent.Set();

                        // Lock thread to avoid run Add task in this thread
                        assertsCheckedEvent.WaitOne();
                    });


                    lockThreadStartedEvent.WaitOne();
                }

                // Act & Assert
                await Assert.ThrowsAsync<TimeoutException>(async () => await _storage.Add(item));
                assertsCheckedEvent.Set();
            }
        }

        [Fact]
        public async Task Add_ReadIsLocked_ShorterThenTimeout()
        {
            // Arrange
            var id = 1;
            var item = new MockEntity(id);
            
            // Act
            using (var testRunThreadEvent = new AutoResetEvent(false))
            {
                using (var lockerRunThreadEvent = new AutoResetEvent(false))
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        _accessLock.EnterReadLock();

                        testRunThreadEvent.Set();
                        lockerRunThreadEvent.WaitOne();

                        _accessLock.ExitReadLock();

                        testRunThreadEvent.Set();
                        
                        // Lock thread to avoid run Add task in this thread
                        lockerRunThreadEvent.WaitOne();
                    });

                    testRunThreadEvent.WaitOne();

                    var addTask = _storage.Add(item, TimeSpan.FromSeconds(1));

                    lockerRunThreadEvent.Set();
                    testRunThreadEvent.WaitOne();

                    await addTask;

                    // Assert
                    Assert.Single(_internalStorageItems.Keys);
                    Assert.Equal(id, _internalStorageItems.Keys.First());
                    Assert.Single(_internalStorageItems.Values);
                    Assert.Equal(item, _internalStorageItems.Values.First());

                    lockerRunThreadEvent.Set();
                }
            }            
        }

        [Fact]
        public async Task Add_WriteIsLocked_LongerThenTimeout()
        {
            // Arrange
            var item = new MockEntity(1);

            // Act
            using (var testRunThreadEvent = new AutoResetEvent(false))
            {
                using (var lockerRunThreadEvent = new AutoResetEvent(false))
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        _accessLock.EnterReadLock();
                        testRunThreadEvent.Set();

                        // Lock thread to avoid run Add task in this thread
                        lockerRunThreadEvent.WaitOne();
                    });

                    // Act && Assert
                    testRunThreadEvent.WaitOne();
                    await Assert.ThrowsAsync<TimeoutException>(async () => await _storage.Add(item));

                    lockerRunThreadEvent.Set();
                }
            }            
        }

        [Fact]
        public async Task Add_WriteIsLocked_ShorterThenTimeout()
        {
            // Arrange
            var id = 1;
            var item = new MockEntity(id);

            // Act
            using (var testRunThreadEvent = new AutoResetEvent(false))
            {
                using (var lockerRunThreadEvent = new AutoResetEvent(false))
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        _accessLock.EnterWriteLock();

                        testRunThreadEvent.Set();
                        lockerRunThreadEvent.WaitOne();

                        _accessLock.ExitWriteLock();

                        testRunThreadEvent.Set();
                        
                        // Lock thread to avoid run addTask in this thread
                        lockerRunThreadEvent.WaitOne();
                    });

                    testRunThreadEvent.WaitOne();

                    var addTask = _storage.Add(item, TimeSpan.FromSeconds(1));

                    lockerRunThreadEvent.Set();
                    testRunThreadEvent.WaitOne();

                    await addTask;

                    // Assert
                    Assert.Single(_internalStorageItems.Keys);
                    Assert.Equal(id, _internalStorageItems.Keys.First());
                    Assert.Single(_internalStorageItems.Values);
                    Assert.Equal(item, _internalStorageItems.Values.First());

                    lockerRunThreadEvent.Set();
                }
            }
        }

        [Fact]
        public async Task Add_CancelToken()
        {
            // Arrange
            var item = new MockEntity(1);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await _storage.Add(item, tokenSource.Token));
        }

        [Fact]
        public async Task Add_SpecifyTimeout()
        {
            // Arrange
            var item = new MockEntity(1);
            var timeout = TimeSpan.FromMilliseconds(500);
            
            // Act
            using (var testRunThreadEvent = new AutoResetEvent(false))
            {
                using (var lockerRunThreadEvent = new AutoResetEvent(false))
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        _accessLock.EnterReadLock();
                        testRunThreadEvent.Set();

                        // Lock thread to avoid run addTask in this thread
                        lockerRunThreadEvent.WaitOne();
                    });

                    testRunThreadEvent.WaitOne();

                    // Act && Assert
                    var stopwatch = Stopwatch.StartNew();
                    await Assert.ThrowsAsync<TimeoutException>(async () => await _storage.Add(item, timeout));
                    Assert.True(stopwatch.Elapsed > timeout, "Elapsed time must be more then timeout. Looks like default timeout is used.");

                    lockerRunThreadEvent.Set();
                }
            }            
        }
    }
}
