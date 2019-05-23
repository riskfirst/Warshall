# Warshall
Calculates reachability of nodes in graphs and distances between them

This C# .NET Core project gives a simple implementation of the Warshall algorithm for determining connectivity between nodes in a graph, yielding the so-called *reachability matrix*. A variation of this algorithm, the Floyd-Warshall algorithm, yields the distances between nodes, the *graph distance matrix*.

This work has applicability in LAZULUM for determining the set of assets which affect other assets, and vice versa. This has relevance in calculating the impacts of corporate actions and contingent lifecycle transactions in extracts.

## Example
The graph (actually two seperate subgraphs):

![alt](https://github.com/riskfirst/Warshall/blob/master/docs/example_graph.jpg "Example graph")

... has an adjacency matrix of

![alt](https://github.com/riskfirst/Warshall/blob/master/docs/example_adjacency.jpg "Example adjacency matrix")

... has a reachability matrix of

![alt](https://github.com/riskfirst/Warshall/blob/master/docs/example_reachability.jpg "Example reachability matrix")

... and a graph distance matrix of

![alt](https://github.com/riskfirst/Warshall/blob/master/docs/example_gdm.jpg "Example graph distance matrix")

## Documentation

See pdf'd Mathematica notebook in docs, or the Mathematica notebook itself. Mathematica has been used as the reference implementation for comparisons. The C# code is basically commented and demonstrates the example shown here and in the notebook.

## Did you know ...

The problem of determining the reachability matrix is technically referred to as determining the transitive closure of the binary relation. Catchy!

