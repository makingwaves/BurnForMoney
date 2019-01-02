import React from "react";

const SVG = ({
  style = {},
  fill = "#9B9B9B",
  width="24px",
  height="19px",
  viewBox="0 0 24 19",
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
  <g id="Symbols" stroke="none" strokeWidth="1" fill="none" fillRule="evenodd" strokeLinecap="round" strokeLinejoin="round">
      <g id="Side-menu-/-Beneficiaries" transform="translate(-20.000000, -136.000000)" stroke={fill}>
          <g id="Group-14" transform="translate(20.000000, 137.000000)">
              <g id="Group-11" transform="translate(0.000000, 0.000000)">
                  <path d="M23.5,15.3488372 C23.5,16.375814 22.605,17.2093023 21.5,17.2093023 L2.5,17.2093023 C1.395,17.2093023 0.5,16.375814 0.5,15.3488372 L0.5,2.3255814 C0.5,1.29860465 1.395,0.465116279 2.5,0.465116279 L21.5,0.465116279 C22.605,0.465116279 23.5,1.29860465 23.5,2.3255814 L23.5,15.3488372 Z" id="Stroke-1"></path>
                  <path d="M0.5,5.11627907 L23.5,5.11627907" id="Stroke-3"></path>
                  <path d="M4.5,2.79069767 C4.5,3.04744186 4.276,3.25581395 4,3.25581395 C3.724,3.25581395 3.5,3.04744186 3.5,2.79069767 C3.5,2.53395349 3.724,2.3255814 4,2.3255814 C4.276,2.3255814 4.5,2.53395349 4.5,2.79069767 Z" id="Stroke-5"></path>
                  <path d="M7.5,2.79069767 C7.5,3.04744186 7.276,3.25581395 7,3.25581395 C6.724,3.25581395 6.5,3.04744186 6.5,2.79069767 C6.5,2.53395349 6.724,2.3255814 7,2.3255814 C7.276,2.3255814 7.5,2.53395349 7.5,2.79069767 Z" id="Stroke-7"></path>
                  <path d="M10.5,2.79069767 C10.5,3.04744186 10.276,3.25581395 10,3.25581395 C9.724,3.25581395 9.5,3.04744186 9.5,2.79069767 C9.5,2.53395349 9.724,2.3255814 10,2.3255814 C10.276,2.3255814 10.5,2.53395349 10.5,2.79069767 Z" id="Stroke-9"></path>
              </g>
          </g>
      </g>
  </g>
  </svg>
);

export default SVG;
