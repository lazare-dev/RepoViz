// src/App.js
import React from 'react';
import { useGraphData } from './hooks/useGraphData';
import MindMapVisualization from './components/MindMapVisualization';
import './App.css'; // Make sure to create this CSS file

function App() {
    const graphData = useGraphData();

    return (
        <div className="App">
            <header className="App-header">
                <h1>Repository Visualization</h1>
            </header>
            <main>
                <MindMapVisualization data={graphData} />
            </main>
        </div>
    );
}

export default App;
