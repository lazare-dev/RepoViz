// src/components/MindMapVisualization.js

import React, { useState, useEffect, useRef } from 'react';
import { DataSet, Network } from 'vis-network/standalone/esm/vis-network';

const MindMapVisualization = ({ data }) => {
    const networkContainerRef = useRef(null);
    const [isLoading, setIsLoading] = useState(true); // Add a loading state

    useEffect(() => {
        if (data.nodes.length > 0 && networkContainerRef.current) {
            const nodes = new DataSet([...new Map(data.nodes.map(node => [node.id, node])).values()]);
            const edges = new DataSet([...new Map(data.edges.map(edge => [`${edge.from}_${edge.to}`, edge])).values()]);

            const networkData = {
                nodes,
                edges,
            };

            const options = {
                nodes: {
                    shape: 'dot',
                    size: 16,
                },
                layout: {
                    hierarchical: true,
                },
                physics: {
                    enabled: true,
                },
            };

            // Initialize the network
            const network = new Network(networkContainerRef.current, networkData, options);

            // Use the "stabilizationIterationsDone" event to know when the network is stabilized
            network.on("stabilizationIterationsDone", function () {
                setIsLoading(false); // Set loading to false once the graph is stabilized
            });
        }
    }, [data]); // Make sure to add isLoading to your dependency array if you plan to use it inside useEffect

    return (
        <div>
            {isLoading ? <div>Loading graph, please wait...</div> : null} {/* Display loading message */}
            <div ref={networkContainerRef} style={{ height: '600px', width: '100%' }} />
        </div>
    );
};

export default MindMapVisualization;
