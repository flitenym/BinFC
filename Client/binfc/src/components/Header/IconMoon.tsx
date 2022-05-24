import { FunctionComponent } from "react";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faMoon } from '@fortawesome/free-solid-svg-icons'
const MoonIcon: FunctionComponent = () => {
    return (
        <FontAwesomeIcon
            style={{ height: "22px", width: "22px", }}
            icon={faMoon} />
    )
}

export default MoonIcon;
