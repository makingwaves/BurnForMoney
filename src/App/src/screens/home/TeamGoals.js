import React, { Component } from 'react';

import './TeamGoals.css';
import team from './img/team.jpg';

class TeamGoals extends Component {
  render() {
    return (
      <div className="TeamGoals">
        <div className="TeamGoals__container container">
          <img src={team} alt="team" className="TeamGoals__image" />
          <h3 className="TeamGoals__header Header">As a team</h3>
          <p>Working together in the office and solving problems side by side</p>
          <p><del>competing</del> cooperating to collect points for our sports activities</p>
          <h3 className="TeamGoals__header Header">... we have two goals</h3>
          <ol className="TeamGoals__list">
            <li className="TeamGoals__list-item">to be active and healthy</li>
            <li className="TeamGoals__list-item">to help others</li>
          </ol>
        </div>

      </div>
    );
  }
}

export default TeamGoals;
