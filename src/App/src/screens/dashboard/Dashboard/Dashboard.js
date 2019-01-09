import React from 'react';
import './Dashboard.css';

import DashboardHeader from '../DashboardHeader/DashboardHeader.js';
import Ranking from '../Ranking/Ranking.js';

const Dashboard = (props) =>{
  return (
    <React.Fragment>
      <DashboardHeader header="Dashboard" />
      <div className="Dashboard-content DashboardGrid">
        <div className="DashboardGridItem DashboardGridItem-summary">
          Summry
        </div>
        <div className="DashboardGridItem DashboardGridItem-results">
          Results
        </div>
        <div className="DashboardGridItem DashboardGridItem-ranking">
          <Ranking ranking={props.ranking} categories={props.categories} rankCategory={props.rankCategory}/>
        </div>
        <div className="DashboardGridItem DashboardGridItem-contribution">
          Activities Contribution
        </div>
      </div>

    </React.Fragment>
  )
}

export default Dashboard;
