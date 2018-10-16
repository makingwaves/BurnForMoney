import React, { Component } from 'react';

import './TeamGoals.css';
import team from '../img/team.jpg';

class TeamGoals extends Component {
  render() {
    return (
      <div className="TeamGoals">
        <div className="TeamGoals__container container">
          <img src={team} alt="team" className="TeamGoals__image" />
          <h3 className="TeamGoals__header Header">We are a team</h3>
          <p>We work together in Making Waves solving problems side by side and creating technological solutions for our clients.</p>
          <p>After work we <del>competing</del> cooperate and train to collect points for our sports activities and to support charity.</p>
        </div>

      </div>
    );
  }
}

export default TeamGoals;
