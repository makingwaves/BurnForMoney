import React from 'react';
import './Dashboard.css';

import DashboardHeader from '../DashboardHeader/DashboardHeader.js';
import RankingList from '../RankingList/RankingList.js';
import RankingFilter from '../RankingFilter/RankingFilter.js';

const Dashboard = (props) =>{
  return (
    <React.Fragment>
      <DashboardHeader header="Dashboard" />
      <div className="Dashboard-content DashboardGrid">
        <div className="DashboardGridItem DashboardGridItem-summary">
          <p>Summry</p>
        </div>
        <div className="DashboardGridItem DashboardGridItem-results">
          Results
        </div>
        <div className="DashboardGridItem DashboardGridItem-ranking">

          <RankingFilter
            rankingCategory={props.rankingCategory}
            setRankinkCategory={props.setRankinkCategory}
            categories={props.categories}
          />
          <h4 className="RankingCategory">{props.rankingCategory}</h4>
          <RankingList
            ranking={props.ranking}
          />
        </div>
        <div className="DashboardGridItem DashboardGridItem-contribution">
          Activities Contribution
        </div>
      </div>

    </React.Fragment>
  )
}

export default Dashboard;
