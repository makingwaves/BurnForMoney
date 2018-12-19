import React, { Component } from 'react';
import './Dashboard.css';

function DashboardHeader(props){
  return (
    <header className="Dashboard__header">
      <h3 className="Dashboard__header-title">{props.header}</h3>
    </header>
  )
}

export default DashboardHeader;
