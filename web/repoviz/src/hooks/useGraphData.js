// src/hooks/useGraphData.js
import { useState, useEffect } from 'react';
import graphData from '../final_graph.json'; // Adjust the path as needed

export const useGraphData = () => {
    const [data, setData] = useState({ nodes: [], edges: [] });

    useEffect(() => {
        // Assuming the structure matches { nodes: [], edges: [] }
        setData(graphData);
    }, []);

    return data;
};
