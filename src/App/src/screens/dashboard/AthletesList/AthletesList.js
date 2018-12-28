import React, { Component } from 'react';
import { Link } from 'react-router-dom';

import './AthletesList.css';

class AthletesList extends Component {

  constructor(props) {
      super(props);
    }

  render() {
    return (
      <div className="AthletesList">
        <ul>
          {this.props.athletes.map( (athlete)=>  <li key={athlete.id} ><Link to={`/dashboard/athlete/${athlete.id}`}>{athlete.firstName} {athlete.lastName}</Link></li>)}
        </ul>
      </div>
    );
  }

}
export default AthletesList;
