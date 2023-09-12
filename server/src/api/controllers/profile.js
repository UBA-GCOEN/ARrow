import userAdminModel from "../models/userAdminModel.js"
import userStudentModel from "../models/userStudentModel.js"
import userStaffModel from "../models/userStaffModel.js"
import userFacultyModel from "../models/userFacultyModel.js"
import userVisitorModel from "../models/userVisitorModel.js"


/**
 * Route: /profile
 * Desc: get profile details 
 */
export const profile = async (req, res) => {

    res.send("Display profile page")
}


/**
 * Route: /profile/edit
 * Desc: request to edit profile
 */
export const editProfile = async (req, res) => {

    const { name, email} = req.body

    const emailDomains = [
        "@gmail.com",
        "@yahoo.com",
        "@hotmail.com",
        "@aol.com",
        "@outlook.com",
        ];

    var result = {}    
    var newResult={}
 
 
 
        //check name length
        if (name.length < 2) {
          return res
            .status(404)
            .json({ message: "Name must be atleast 2 characters long." });
        }
 
 
 
        // check email format
        if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
          return res.status(404).json({
         message: "Please enter a valid email address",
       })};



    /**
     * getting remaing fields 
     * from requests.
     */
     var {
        branch,
        subjects,
        intrest,
        enrollNo,
        mobile,
        designation,
        bio,
        education,
        intrest,
        mobile
     } = req.body


     // conditions to figure out role
    if(req.role === 'admin'){

           var adminObj = {
            name:name,
            email:email,
            branch:branch,
           }
           
           // update userAdmin in database 
             result = await userAdminModel.updateOne(adminObj);

             
         newResult = await userAdminModel.findOne(adminObj)

    }
    else if(req.role === 'student'){

        var studentObj = {
            name:name,
            email:email,
            branch:branch,
            intrest:intrest,
            enrollNo:enrollNo,
            mobile:mobile
         }


         // update userStudent in database 
            result = await userStudentModel.updateOne(studentObj);

         
         newResult = await userStudentModel.findOne(studentObj)
    }
    else if(req.role === 'staff'){
          
        var staffObj = {
            name:name,
            email:email,
            branch:branch,
            designation:designation,
            bio:bio,
            mobile:mobile
         }


        //update staff in database
            result = await userStaffModel.updateOne(staffObj);

         newResult = await userStaffModel.findOne(staffObj)
    }
    else if(req.role === 'faculty'){

        var facultyObj = {
            name:name,
            email:email,
            branch:branch,
            subjects:subjects,
            designation:designation,
            education:education,
            bio:bio,
            intrest:intrest,
            mobile:mobile
         }

        //update faculty in database
        result = await userFacultyModel.updateOne(facultyObj);

         
         newResult = await userFacultyModel.findOne(facultyObj)
    }
    else if(req.role == 'visitor'){
         
        var visitorObj = {
            name:name,
            email:email,
            bio:bio
         }

        //update visitor in database
        result = await userVisitorModel.updateOne(visitorObj);

         newResult = await userVisitorModel.findOne(visitorObj)
    }



    if(result){

        
        res.json({
            msg: "user "+req.role+" edited successfully",
            user: newResult
    })
     }
    
}
