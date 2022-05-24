import { FunctionComponent } from "react";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSun } from '@fortawesome/free-solid-svg-icons'
const MoonIcon: FunctionComponent = () => {
    return (
        <FontAwesomeIcon
            style={{ height: "22px", width: "22px", }}
            icon={faSun} />
    )
}

export default MoonIcon;
