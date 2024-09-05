using HarmonyLib;
using SappUnityUtils.Graphs;
using SDDebug.MenuGUI;
using System.Collections.Generic;
using System.Reflection;

namespace SDDebug.Patches
{
    [HarmonyPatch(typeof(CarAnalyzer2))]
    internal class CarAnalyzer2Patch
    {
        static MethodInfo arePartConfigsConectedTagMethod;
        static MethodInfo isConnectorTypeTupleInListMethod;

        [HarmonyPatch("removeEdgesBetweenGearsThatAreInDifferentParents")]
        [HarmonyPrefix]
        static bool removeEdgesBetweenGearsThatAreInDifferentParentsPatch() => !SDBuildMenu.disableErrors.Value;

        /*[HarmonyPatch("arePartConfigsConectedTag")]
        [HarmonyPrefix]
        static bool arePartConfigsConectedTagPatch(ref bool __result)
        {
            if (SDBuildMenu.disableErrors.Value)
                __result = true;
            return !SDBuildMenu.disableErrors.Value;
        }*/

        [HarmonyPatch("graphFromPartConfigurations")]
        [HarmonyPrefix]
        static bool graphFromPartConfigurationsPatch(ref GraphTag<PartConfiguration, GraphEdgeMeta> __result,
                           PartConfiguration[] c,
                           Dictionary<PartConfiguration, PartConfiguration> mapToOriginals,
                           List<CarError> carErrors,
                           List<(ConnectorType, ConnectorType)> skipConnectionsBetweenConnectorTypes)
        {
            if (!SDBuildMenu.disableErrors.Value)
                return true;

            // Initialize reflection methods
            if (arePartConfigsConectedTagMethod == null)
                arePartConfigsConectedTagMethod = typeof(CarAnalyzer2).GetMethod("arePartConfigsConectedTag", BindingFlags.NonPublic | BindingFlags.Static);

            if (isConnectorTypeTupleInListMethod == null)
                isConnectorTypeTupleInListMethod = typeof(CarAnalyzer2).GetMethod("isConnectorTypeTupleInList", BindingFlags.NonPublic | BindingFlags.Static);

            GraphTag<PartConfiguration, GraphEdgeMeta> graph = new GraphTag<PartConfiguration, GraphEdgeMeta>();
            for (int index = 0; index < c.Length; ++index)
                graph.AddNode(c[index]);

            for (int index1 = 0; index1 < c.Length; ++index1)
            {
                for (int index2 = index1 + 1; index2 < c.Length; ++index2)
                {
                    ConnectorType[] conTypes;

                    // Use reflection to invoke arePartConfigsConectedTag
                    var areConnectedParams = new object[] { c[index1], c[index2], null };
                    bool areConnected = (bool)arePartConfigsConectedTagMethod.Invoke(null, areConnectedParams);
                    conTypes = (ConnectorType[])areConnectedParams[2];

                    if (areConnected)
                    {
                        bool isInList = (bool)isConnectorTypeTupleInListMethod.Invoke(null, new object[] { conTypes[0], conTypes[1], skipConnectionsBetweenConnectorTypes });

                        if (!isInList)
                        {
                            if (!graph.ExistsEdgeFromTo(graph.GetNode(c[index1]), graph.GetNode(c[index2])))
                            {
                                GraphEdgeMeta tag1 = new GraphEdgeMeta();
                                GraphEdgeMeta tag2 = new GraphEdgeMeta();
                                graph.AddEdge(graph.GetNode(c[index1]), graph.GetNode(c[index2]), tag1);
                                graph.AddEdge(graph.GetNode(c[index2]), graph.GetNode(c[index1]), tag2);
                            }
                            EdgeTag<PartConfiguration, GraphEdgeMeta> edgeBetween1 = (EdgeTag<PartConfiguration, GraphEdgeMeta>)graph.GetEdgeBetween(graph.GetNode(c[index1]), graph.GetNode(c[index2]));
                            EdgeTag<PartConfiguration, GraphEdgeMeta> edgeBetween2 = (EdgeTag<PartConfiguration, GraphEdgeMeta>)graph.GetEdgeBetween(graph.GetNode(c[index2]), graph.GetNode(c[index1]));
                            edgeBetween1.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoConnectors(conTypes[0], conTypes[1]));
                            edgeBetween2.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoConnectors(conTypes[1], conTypes[0]));
                        }
                    }
                    if (Part.MakePart(c[index1].partType).IsGear && Part.MakePart(c[index2].partType).IsGear)
                    {
                        float gearRatio;
                        float gearRatioVisual;
                        bool turnsOtherInOppDir;
                        if (GearsAnalyzer.AreGearsConnected(c[index1], c[index2], out gearRatio, out gearRatioVisual, out turnsOtherInOppDir))
                        {
                            if (!graph.ExistsEdgeFromTo(graph.GetNode(c[index1]), graph.GetNode(c[index2])))
                            {
                                GraphEdgeMeta tag3 = new GraphEdgeMeta();
                                GraphEdgeMeta tag4 = new GraphEdgeMeta();
                                graph.AddEdge(graph.GetNode(c[index1]), graph.GetNode(c[index2]), tag3);
                                graph.AddEdge(graph.GetNode(c[index2]), graph.GetNode(c[index1]), tag4);
                            }
                            EdgeTag<PartConfiguration, GraphEdgeMeta> edgeBetween3 = (EdgeTag<PartConfiguration, GraphEdgeMeta>)graph.GetEdgeBetween(graph.GetNode(c[index1]), graph.GetNode(c[index2]));
                            EdgeTag<PartConfiguration, GraphEdgeMeta> edgeBetween4 = (EdgeTag<PartConfiguration, GraphEdgeMeta>)graph.GetEdgeBetween(graph.GetNode(c[index2]), graph.GetNode(c[index1]));
                            edgeBetween3.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoTransmission(gearRatio, gearRatioVisual, turnsOtherInOppDir));
                            edgeBetween4.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoTransmission(1f / gearRatio, 1f / gearRatioVisual, turnsOtherInOppDir));
                        }
                    }
                    else if (PowertrainAnalyzer.isPowertrainPart(c[index1]) && PowertrainAnalyzer.isPowertrainPart(c[index2]))
                    {
                        EdgeTag<PartConfiguration, GraphEdgeMeta> edgeBetween5 = (EdgeTag<PartConfiguration, GraphEdgeMeta>)graph.GetEdgeBetween(graph.GetNode(c[index1]), graph.GetNode(c[index2]));
                        EdgeTag<PartConfiguration, GraphEdgeMeta> edgeBetween6 = (EdgeTag<PartConfiguration, GraphEdgeMeta>)graph.GetEdgeBetween(graph.GetNode(c[index2]), graph.GetNode(c[index1]));
                        if ((Edge<PartConfiguration>)edgeBetween5 != (Edge<PartConfiguration>)null && (Edge<PartConfiguration>)edgeBetween6 != (Edge<PartConfiguration>)null)
                        {
                            float gearRatio1To2;
                            float gearRatioVisual1To2;
                            if (GearsAnalyzer.IsConnectionBetweenAxeAndPlanetGears(graph, c[index1], c[index2], out gearRatio1To2, out gearRatioVisual1To2))
                            {
                                edgeBetween5.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoTransmission(gearRatio1To2, gearRatioVisual1To2, false));
                                edgeBetween6.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoTransmission(1f / gearRatio1To2, 1f / gearRatioVisual1To2, false));
                            }
                            else
                            {
                                edgeBetween5.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoTransmission(1f, 1f, false));
                                edgeBetween6.Tag.AddMetaInfo((GraphEdgeInfo)new GraphEdgeInfoTransmission(1f, 1f, false));
                            }
                        }
                    }
                }
            }

            __result = graph;
            return false;
        }
    }
}
