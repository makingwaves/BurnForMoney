import React from 'react';
import { Link } from 'react-router-dom';
import './Dashboard.css';

import DashboardHeader from '../DashboardHeader/DashboardHeader.js';
import RankingList from '../RankingList/RankingList.js';
import RankingFilter from '../RankingFilter/RankingFilter.js';
import PointsAndMoney from '../PointsAndMoney/PointsAndMoney.js';
import ProgressBonus from '../ProgressBonus/ProgressBonus.js';
import ActivitiesContribution from '../ActivitiesContribution/ActivitiesContribution.js';

const Dashboard = (props) =>{
  return (
    <React.Fragment>
      <DashboardHeader header="Dashboard" />
      <div className="Dashboard-content DashboardGrid">

        <div className="DashboardGridItem DashboardGridItem-progress">
          <ProgressBonus />
        </div>

        <div className="DashboardGridItem DashboardGridItem-results">
          <PointsAndMoney />
        </div>

        <div className="DashboardGridItem DashboardGridItem-ranking">
          <div className="DashboardGridItem-rankingHeader">
            <h4 className="DashboardGridItem-rankingHeader-title">Rank</h4>
            <Link to="/dashboard/participants" className="DashboardGridItem-rankingHeader-link">All participants</Link>
          </div>
          <div className="DashboardGridItem DashboardGridItem-rankingContent">
            <RankingFilter
              rankingCategory={props.rankingCategory}
              setRankinkCategory={props.setRankinkCategory}
              categories={props.categories}
            />
            <h4 className="RankingCategory">{props.rankingCategory}</h4>
            <RankingList
              ranking={props.ranking}
              rankingLoading={props.rankingLoading}
            />
          </div>
        </div>

        <div className="DashboardGridItem DashboardGridItem-contribution">
            <ActivitiesContribution
              categories={props.categories}
             />
        </div>
      </div>

    </React.Fragment>
  )
}

export default Dashboard;
