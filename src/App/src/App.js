import React from 'react';
import {
  BrowserRouter as Router,
  Route,
  Switch
} from 'react-router-dom';

import { BrowserRouter } from "react-router-dom";
import { Routes } from "./routes/routes";

function App(){
 
  return (  
      <BrowserRouter children={Routes} basename={"/"} />
  );
}

export default App;
