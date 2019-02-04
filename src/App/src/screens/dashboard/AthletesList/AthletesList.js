import React from 'react';
import { Link } from 'react-router-dom';

import './AthletesList.css';

const AthletesList = (props) => {
  console.log("PROPS", props)
  return (
    <div className="AthletesList">
      <ul>
        {props.athletes.map( (athlete)=>  <li key={athlete.id} ><Link to={`/dashboard/athlete/${athlete.id}`}>{athlete.firstName} {athlete.lastName}</Link></li>)}
      </ul>
    </div>
  );
}

export default AthletesList;
