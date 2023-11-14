import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";
import { addUserToProcedure, getProcedureUsers } from "../../../api/api";

const PlanProcedureItem = ({ planId, procedure, users }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(() => {
        (async () => {
            var procedureUsers = await getProcedureUsers(planId, procedure.procedureId);
            var users = [];
            procedureUsers.map((u) => users.push({ label: u.user.name, value: u.user.userId }));
            console.log(planId);
            setSelectedUsers(users);
        })();
    }, [planId, procedure.procedureId]);

    const handleAssignUserToProcedure = async (e) => {
        setSelectedUsers(e);
        var userIds = e.map(function (item) {
            return item.value;
        });
        console.log(procedure);
        await addUserToProcedure(planId, procedure.procedureId, userIds);
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
