import React from "react";

const SVG = ({
  style = {},
  fill = "#122833",
  width= "31px",
  height="31px",
  viewBox="0 0 31 31",
  className = ""
}) => (
  <svg
    width={width}
    style={style}
    height={width}
    viewBox={viewBox}
    xmlns="http://www.w3.org/2000/svg"
    className={`svg-icon ${className || ""}`}
    xmlnsXlink="http://www.w3.org/1999/xlink"
  >
  <g id="Symbols" stroke="none" strokeWidth="1" fill="none" fillRule="evenodd">
      <g id="Side-menu-/-dashboard" transform="translate(-19.000000, -970.000000)" fill={fill} fillRule="nonzero">
          <g id="Group-4" transform="translate(11.000000, 963.000000)">
              <g id="Group-3" transform="translate(8.000000, 7.000000)">
                  <path d="M15.5,31 C6.93958638,31 0,24.0604136 0,15.5 C0,6.93958638 6.93958638,0 15.5,0 C24.0604136,0 31,6.93958638 31,15.5 C31,24.0604136 24.0604136,31 15.5,31 Z M17.3015184,10 L13.6984816,10 L13.6984816,13.8419958 L10,13.8419958 L10,17.1580042 L13.6984816,17.1580042 L13.6984816,21 L17.3015184,21 L17.3015184,17.1580042 L21,17.1580042 L21,13.8419958 L17.3015184,13.8419958 L17.3015184,10 Z" id="Combined-Shape"></path>
              </g>
          </g>
      </g>
  </g>
  </svg>
);

export default SVG;
