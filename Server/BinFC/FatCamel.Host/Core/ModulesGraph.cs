using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FatCamel.Host.Core
{
    /// <summary>
    /// Граф модулей
    /// </summary>
    public class ModulesGraph : IEnumerable<InternalModule>
    {
        /// <summary>
        /// 
        /// </summary>
        public struct Edge
        {
            public string From { get; set; }
            public string To { get; set; }

            public override int GetHashCode() => From.GetHashCode() ^ To.GetHashCode();

            public override bool Equals(object? obj)
            {
                var other = obj as Edge?;
                if (!other.HasValue) return false;

                return From == other.Value.From && To == other.Value.To;
            }
        }

        /// <summary>
        /// Модуль - узел графа
        /// </summary>
        public class Node
        {
            private readonly ModulesGraph _graph;

            private IEnumerable<Node>? _dependantNodes;

            /// <summary>
            /// Объект модуля
            /// </summary>
            public InternalModule Module { get; }

            /// <summary>
            /// Список зависимых модулей
            /// </summary>
            public IEnumerable<Node>? DependantNodes
            {
                get
                {
                    if (_dependantNodes == null)
                    {
                        _dependantNodes = _graph._edges.Where(e => e.From == Module.Metadata.Key).Select(e => new Node(_graph, _graph.First(m => m.Metadata.Key == e.To)));
                    }

                    return _dependantNodes;
                }
            }

            internal Node(ModulesGraph graph, InternalModule module)
            {
                _graph = graph;
                Module = module;
            }
        }

        private readonly List<InternalModule> _nodes = new List<InternalModule>();

        private readonly List<Edge> _edges = new List<Edge>();

        private readonly Dictionary<string, List<string>> _missingDependencies = new Dictionary<string, List<string>>();

        private readonly List<Node> _roots = new List<Node>();

        /// <summary>
        /// Корневые модули, модули без зависимостей
        /// </summary>
        public IEnumerable<Node> Roots => _roots;

        /// <summary>
        /// Список всех зависимостей модулей ввиде линеного списка
        /// </summary>
        public IEnumerable<Edge> Edges => _edges;

        /// <summary>
        /// Список не разрешённых зависимостей и модулей их требующие
        /// </summary>
        public IReadOnlyDictionary<string, List<string>> MissingDependencies => _missingDependencies;

        /// <summary>
        /// Получает список, отсортированный по зависимостям
        /// </summary>
        public static IEnumerable<ModuleMetadata> GetOrderedSources(IEnumerable<ModuleMetadata> source)
        {
            Dictionary<ModuleMetadata, int> orderedSource = new();

            foreach (var md in source)
            {
                orderedSource.Add(md, 0);
            }

            void checkForCycleDep(ModuleMetadata moduleForCheck, ModuleMetadata nextModule)
            {
                if (nextModule.Dependencies?.Any(p => p.Key == moduleForCheck.Name) ?? false)
                {
                    List<string> errorList = new();
                    errorList.Add(moduleForCheck.Name);
                    errorList.AddRange(nextModule.Dependencies.Select(p => p.Key));
                    throw new CycleReferenceException(errorList);
                }

                var list = source.Where(m => nextModule.Dependencies?.Any(d => d.Key == m.Name) ?? false);

                if (list != null && list.Any())
                {
                    foreach (var item in list)
                    {
                        checkForCycleDep(moduleForCheck, item);
                    }
                }
            }

            void calculateWeight(ModuleMetadata moduleMetadata, Dictionary<ModuleMetadata, int> list)
            {
                var mdDeps = list
                    .Where(p => p.Key.Dependencies?
                        .Any(d => d.Key == moduleMetadata.Name) ?? false)
                    .ToList();

                foreach (var mdDep in mdDeps)
                {
                    list[mdDep.Key]++;
                    calculateWeight(mdDep.Key, list);
                }
            }

            foreach (var md in orderedSource)
            {
                checkForCycleDep(md.Key, md.Key);
                calculateWeight(md.Key, orderedSource);
            }

            return orderedSource
                .OrderBy(p => p.Value)
                .ThenBy(p => p.Key.Name)
                .Select(p => p.Key);
        }

        /// <summary>
        /// Граф модулей
        /// </summary>
        public ModulesGraph(IEnumerable<ModuleMetadata> source)
        {
            var sourceList = source.ToList();
            foreach (var md in GetOrderedSources(sourceList))
            {
                var mod = new InternalModule(md);

                _missingDependencies.Remove(md.Key);

                _nodes.Add(mod);

                if ((md.Dependencies?.Count ?? 0) == 0)
                {
                    _roots.Add(new Node(this, mod));
                    continue;
                }

                foreach (var dep in md.Dependencies!)
                {
                    var key = $"{dep.Key}:{dep.Value}";
                    _edges.Add(new Edge
                    {
                        From = key,
                        To = md.Key
                    });

                    if (!_nodes.Any(n => n.Metadata.Key == key))
                    {
                        if (!_missingDependencies.TryGetValue(key, out var list))
                            _missingDependencies.Add(key, new List<string>(new[] { md.Name }));
                        else
                            _missingDependencies[key].Add(md.Name);
                    }
                }
            }
        }

        public IEnumerator<InternalModule> GetEnumerator() => _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Проход по узлам графа и выполнение действия
        /// </summary>
        /// <remarks>Действие для модуля с зависимостью будет выполнено только после всех модулей от он зависит</remarks>
        /// <param name="action">Действие выполняемое для каждого узла</param>
        public void TraverseAndExecute(Action<Node> action)
        {
            Queue<ModulesGraph.Node> nodes = new Queue<ModulesGraph.Node>(Roots);
            List<string> loaded = new List<string>();

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();

                var deps = node.Module.Metadata.Dependencies?.Select(s => s.Key);
                if (deps?.Any() == true)
                {
                    var skip = false;
                    foreach (var dep in deps)
                    {
                        if (!loaded.Contains(dep))
                        {
                            nodes.Enqueue(node);
                            skip = true;
                            break;
                        }
                    }

                    if (skip)
                        continue;
                }

                action(node);
                loaded.Add(node.Module.Name);

                if (node.DependantNodes?.Any() == true)
                    foreach (var depNode in node.DependantNodes)
                    {
                        if (!nodes.Any(n => n.Module.Name == depNode.Module.Name))
                            nodes.Enqueue(depNode);
                    }
            }
        }
    }
}