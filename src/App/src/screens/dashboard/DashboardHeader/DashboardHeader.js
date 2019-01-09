import React from 'react';
import './DashboardHeader.css';

const DashboardHeader = (props) =>{
  return (
    <header className="DashboardHeader">
      <h3 className="DashboardHeader-title">{props.header}</h3>
    </header>
  )
}

export default DashboardHeader;
